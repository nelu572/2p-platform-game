using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;
    private IDamageable _playerDamageable;
    private IDamageable _enemyDamageable;

    void Awake()
    {
        _playerDamageable = _player.GetComponent<IDamageable>();
        _enemyDamageable = _enemy.GetComponent<IDamageable>();

        _playerDamageable.TeamId = 1;
        _enemyDamageable.TeamId = 2;
    }
}
