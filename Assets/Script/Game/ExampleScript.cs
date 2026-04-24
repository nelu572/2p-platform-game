using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;

    void Awake()
    {
        _player.GetComponent<IDamageable>().TeamId = 1;
        _enemy.GetComponent<IDamageable>().TeamId = 2;
    }
}
