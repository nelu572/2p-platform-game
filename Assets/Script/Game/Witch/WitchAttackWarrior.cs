using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(ChargeInputHandler))]
[RequireComponent(typeof(ObjectPoolManager))]
public class WitchAttackWarrior : MonoBehaviour, IAttackController, IChargeable
{
    [Header("일반 공격")]
    [SerializeField] private float _throwPower = 0;
    [SerializeField] private float _throwMaxPower = 10f;
    [SerializeField] private string[] _posions = { "Pain", "Poison", "Slow" };
    [SerializeField] private GameObject _posionObject;
    public bool IsCharging { get; set; }
    [Header("스킬")]
    // 1번은 고통, 2번은 독, 3번은 구속
    [SerializeField] private int _posionIndex;
    [SerializeField] private int _postionMaxIndex = 3;

    [SerializeField] private LayerMask _attackLayerMask;
    private PlayerController _playerController;
    private PlayerStat _playerStat;
    private ObjectPoolManager _objectPoolManager;// 오브젝트 풀링 스크립트
    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator _animator;
    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();
        _objectPoolManager = GetComponent<ObjectPoolManager>();
        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator = GetComponent<Animator>();
        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;
    }

    void Update()
    {
        if (IsCharging)
        {
            _throwPower += Time.deltaTime; // 충전 속도 조절
            _throwPower = Mathf.Min(_throwPower, _throwMaxPower); // 최대 충전 제한
        }
    }

    public void Attack()
    {
        if (_playerController.IsGrounded)
        {
            //_animator.SetTrigger("ReadyThrow");
            IsCharging = true;
        }
        else
        {
            //_animator.SetTrigger("JumpAction");
            JumpAttack();
        }
    }

    public void ReleaseAttack()
    {
        IsCharging = false;
        _objectPoolManager.Get(_posions[_posionIndex]);
        _playerStat.AttackCooltime = _playerStat.AttackCooltimeMax; // 공격 쿨타임 초기화
        
    }

    public void JumpAttack()
    {

    }

    public void Skill()
    {
        _posionIndex++;
        _posionIndex = Mathf.Min(_posionIndex, _postionMaxIndex); // 최대 포션 인덱스 제한
    }

    public void ReleaseCharge(string actionName)
    {
        switch (actionName)
        {
            case "Attack": ReleaseAttack(); break;
        }
    }
}