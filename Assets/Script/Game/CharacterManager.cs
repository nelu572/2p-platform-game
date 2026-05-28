using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{

    [Header("Data")]
    [SerializeField] private GameDataSO _gameData;
    [SerializeField] private InGameManager _inGameManager;

    [Header("Spawn Point")]
    [SerializeField] private Transform _player1SpawnPoint;
    [SerializeField] private Transform _player2SpawnPoint;

    [Header("Fallback Selection (Character Select 미완성 대응)")]
    [SerializeField] private int _fallbackP1CharacterIndex = 0;
    [SerializeField] private int _fallbackP2CharacterIndex = 1;

    private GameObject _player1;
    private GameObject _player2;
    public void SpawnPlayers()
    {
        _player1 = SpawnSinglePlayer(1, "Player1", ResolveCharacterIndex(isPlayer1: true), _player1SpawnPoint);
        _player2 = SpawnSinglePlayer(2, "Player2", ResolveCharacterIndex(isPlayer1: false), _player2SpawnPoint);

        if (_inGameManager == null)
            _inGameManager = FindFirstObjectByType<InGameManager>();

        if (_inGameManager != null)
        {
            PlayerStat p1 = _player1 != null ? _player1.GetComponent<PlayerStat>() : null;
            PlayerStat p2 = _player2 != null ? _player2.GetComponent<PlayerStat>() : null;
            _inGameManager.InitializePlayers(p1, p2);
        }
    }

    private int ResolveCharacterIndex(bool isPlayer1)
    {
        if (_gameData == null)
            return isPlayer1 ? _fallbackP1CharacterIndex : _fallbackP2CharacterIndex;

        return isPlayer1 ? _gameData.p1CharacterIndex : _gameData.p2CharacterIndex;
    }

    private GameObject SpawnSinglePlayer(int teamId, string actionMapName, int characterIndex, Transform spawnPoint)
    {
        if (_gameData == null || _gameData.characterPrefabs == null || _gameData.characterPrefabs.Length == 0)
        {
            Debug.LogError("GameDataSO 또는 characterPrefabs가 비어 있어 플레이어를 생성할 수 없습니다.");
            return null;
        }

        int safeIndex = Mathf.Clamp(characterIndex, 0, _gameData.characterPrefabs.Length - 1);
        GameObject prefab = _gameData.characterPrefabs[safeIndex];
        if (prefab == null)
        {
            Debug.LogError($"characterPrefabs[{safeIndex}]가 비어 있습니다.");
            return null;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        GameObject spawned = Instantiate(prefab, spawnPosition, spawnRotation);
        spawned.name = $"P{teamId}_{prefab.name}";

        PlayerStat playerStat = spawned.GetComponent<PlayerStat>();
        if (playerStat != null)
            playerStat.TeamId = teamId;

        PlayerInput playerInput = spawned.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.SwitchCurrentActionMap(actionMapName);
        else
            Debug.LogWarning($"{spawned.name}에 PlayerInput 컴포넌트가 없어 액션맵을 분리할 수 없습니다.");

        return spawned;
    }
}
