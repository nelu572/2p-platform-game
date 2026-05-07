using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;
    private PlayerStat _playerStat;
    private PlayerStat _enemyStat;
    [SerializeField] private int _playerTeamId;
    [SerializeField] private int _enemyTeamId;

    void Awake()
    {
        _playerStat = _player.GetComponent<PlayerStat>();
        _enemyStat = _enemy.GetComponent<PlayerStat>();

        _playerStat.TeamId = 1;
        _enemyStat.TeamId = 2;
        _playerTeamId = _playerStat.TeamId;
        _enemyTeamId = _enemyStat.TeamId;

    }
}
