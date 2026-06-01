using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private RoundEndUI _roundEndUI;
    [SerializeField] private CameraMove _cameraMove;

    //자동으로 할당됨
    [SerializeField] private PlayerStat _playerStat1;
    [SerializeField] private PlayerStat _playerStat2;
    [SerializeField] private GameObject _player1;
    [SerializeField] private GameObject _player2;

    private bool isRoundOver;

    void Awake()
    {
        _characterManager.SpawnPlayers();
    }

    void Update()
    {
        if (isRoundOver || _playerStat1 == null || _playerStat2 == null)
            return;

        if (_playerStat1.Hp <= 0) PlayerDead(_playerStat1);
        if (_playerStat2.Hp <= 0) PlayerDead(_playerStat2);
    }

    //캐릭터가 생성될때 실행
    public void InitializePlayers(PlayerStat playerStat1, PlayerStat playerStat2)
    {
        if (playerStat1 == null || playerStat2 == null)
        {
            Debug.LogError("InitializePlayers: 스텟이 비어있습니다");
            return;
        }
        _playerStat1 = playerStat1;
        _playerStat2 = playerStat2;
        _player1 = playerStat1.gameObject;
        _player2 = playerStat2.gameObject;
        _playerStat1.TeamId = 1;
        _playerStat2.TeamId = 2;
        InitializeCameraTargets();
        isRoundOver = false;
    }

    public void PlayerDead(PlayerStat deadPlayer)
    {
        if (deadPlayer.Life > 0)
        {
            Debug.Log($"player{deadPlayer.TeamId}사망");
            /// TODO: 캐릭터에 맞는 유령 생성 로직 작성 필요 
            deadPlayer.Revive();
            RespawnPlayer(deadPlayer);
            Debug.Log("부활됨");
        }
        else
        {
            PlayerStat winnerPlayer = (deadPlayer == _playerStat1) ? _playerStat2 : _playerStat1;
            SelectWinner(winnerPlayer, deadPlayer);
        }
    }

    private void RespawnPlayer(PlayerStat player)
    {
        if (player == null)
            return;

        Vector3 respawnPosition = _characterManager != null
            ? _characterManager.GetSpawnPosition(player.TeamId)
            : Vector3.zero;

        player.transform.position = respawnPosition;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void InitializeCameraTargets()
    {
        if (_cameraMove == null)
            _cameraMove = FindFirstObjectByType<CameraMove>();

        if (_cameraMove != null)
            _cameraMove.SetTargets(_player1.transform, _player2.transform);
    }

    public void SelectWinner(PlayerStat winnerPlayer, PlayerStat loserPlayer)
    {
        if (isRoundOver) return;
        Debug.Log($"플레이어{winnerPlayer.TeamId} 승리");
        StartCoroutine(RoundEnd(winnerPlayer, loserPlayer));
    }
    private IEnumerator RoundEnd(PlayerStat winner, PlayerStat loser)
    {
        isRoundOver = true;
        SetPlayersMovementEnabled(false);

        //  슬로우모션 시작
        float originalFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = 0.2f;        // 속도를 20%로 줄임
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
        _roundEndUI?.Show();
        // TODO: RoundEndUI Show 완료 후 버튼 등장 애니메이션을 실행하고, 2초 뒤 플레이어 입력을 다시 켜기.

        // 실제 시간 기준으로 3초 대기 (Unscaled = timeScale 영향 안 받음)
        yield return new WaitForSecondsRealtime(3f);

        //  속도 원복
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        // TODO: 결과 UI 표시
        StartCoroutine(CreateResultUI());
    }
    private IEnumerator CreateResultUI()
    {
        yield return null;
        //TODO 결과 화면 SetActive(true) & 결과 화면에 들어가야될 정보 추가
    }

    private void SetPlayersMovementEnabled(bool enabled)
    {
        SetPlayerMovementEnabled(_player1, enabled);
        SetPlayerMovementEnabled(_player2, enabled);
    }

    private void SetPlayerMovementEnabled(GameObject player, bool enabled)
    {
        if (player == null)
            return;

        ChargeInputHandler chargeInputHandler = player.GetComponent<ChargeInputHandler>();
        if (chargeInputHandler != null)
            chargeInputHandler.enabled = enabled;

        PlayerInputHandler inputHandler = player.GetComponent<PlayerInputHandler>();
        if (inputHandler != null)
        {
            if (!enabled)
                inputHandler.ResetInput();

            inputHandler.enabled = enabled;
        }

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = enabled;
    }
}
