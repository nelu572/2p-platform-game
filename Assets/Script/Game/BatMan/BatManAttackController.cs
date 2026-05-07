using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(ChargeInputHandler))]
public class BatManAttackController : MonoBehaviour, IAttackController, IChargeable
{
    [Header("일반 공격")]

    [Header("스킬")]

    private PlayerController _playerController;
    private PlayerStat _playerStat;
    public bool IsCharging { get; set; }

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();
    }

    void Update()
    {

    }

    public void Attack()
    {

    }

    public void ReleaseCharge(string actionName)
    {

    }

    public void Skill()
    {

    }
}
