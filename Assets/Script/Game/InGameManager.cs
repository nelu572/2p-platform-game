using System.Collections;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private PlayerStat _playerStat1;
    [SerializeField] private PlayerStat _playerStat2;
    [SerializeField] private CharacterManager _characterManager;

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;

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
    public void InitializePlayers(PlayerStat player1, PlayerStat player2)
    {
        _playerStat1 = player1;
        _playerStat2 = player2;
        _playerStat1.TeamId = 1;
        _playerStat2.TeamId = 2;
        isRoundOver = false;
    }

    public void PlayerDead(PlayerStat deadPlayer)
    {
        if (deadPlayer.Life > 0)
        {
            Debug.Log($"player{deadPlayer.TeamId}사망");
            /// TODO: 캐릭터에 맞는 유령 생성 로직 작성 필요 
            deadPlayer.Revive();
            Debug.Log("부활됨");
        }
        else
        {
            PlayerStat winnerPlayer = (deadPlayer == _playerStat1) ? _playerStat2 : _playerStat1;
            SelectWinner(winnerPlayer, deadPlayer);
        }
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

        //  슬로우모션 시작
        Time.timeScale = 0.2f;        // 속도를 20%로 줄임
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 실제 시간 기준으로 3초 대기 (Unscaled = timeScale 영향 안 받음)
        yield return new WaitForSecondsRealtime(3f);

        //  속도 원복
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // TODO: 결과 UI 표시
        StartCoroutine(CreateResultUI());
    }
    private IEnumerator CreateResultUI()
    {
        yield return null;
        //TODO 결과 화면 SetActive(true) & 결과 화면에 들어가야될 정보 추가
    }
}
