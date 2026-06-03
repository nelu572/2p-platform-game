using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ChargeInputHandler))]
public class BatManAttackController : BaseAttackController, IChargeable
{
    [Header("일반 공격")]
    [SerializeField] private float _chargingTime = 0f;
    [SerializeField] private float _chargingMaxTime = 3f;
    [SerializeField] private float _knockbackPower = 10f;
    [SerializeField] private Vector2 _attackSize = new Vector2(3f, 2f);
    [SerializeField] private LayerMask _attackLayerMask;
    [SerializeField] private int[] _chargeMultiplier = { 1, 2, 3, 4 };
    [SerializeField] private BoxCollider2D _attackOffset;
    [Header("스킬")]
    //기본값으로 50%의 배율로 설정
    [SerializeField] private float _speedUpScale = 0.5f;
    private float _plusMoveSpeed;
    //10초를 기준으로
    [SerializeField] private float _duration = 10f;
    [SerializeField] private bool _isSpeedUp = false;

    //private Animator _animator;
    public bool IsCharging { get; set; }

    protected override void Awake()
    {
        base.Awake();
        if (_attackOffset == null)
            _attackOffset = BodyCollider;
        //_animator = GetComponent<Animator>();

        SetupContactFilter(_attackLayerMask);
    }

    void Update()
    {
        if (IsCharging)
        {
            _chargingTime += Time.deltaTime;
            _chargingTime = Mathf.Min(_chargingTime, _chargingMaxTime);
        }

        if(_isSpeedUp)
        {
            if(_duration > 0f)
                _duration -= Time.deltaTime;
            else
            {
                _isSpeedUp = false;
                PlayerStat.MoveSpeed -= _plusMoveSpeed;
            }
        }
    }

    public override void Attack()
    {
        if (IsCharging) return;
        if (IsAttackOnCooldown()) return; // 추가

        IsCharging = true;
        //_animator.SetTrigger("Chargeing");
    }

    public void ReleaseAttack()
    {
        if (!IsCharging) return;

        int level = Mathf.Clamp(Mathf.FloorToInt(_chargingTime), 0, _chargeMultiplier.Length - 1);
        int damage = PlayerStat.AttackDamage * _chargeMultiplier[level];
        float knockback = _knockbackPower * _chargeMultiplier[level];

        Vector2 facingDir = GetFacingDirection();
        Vector2 origin = GetHorizontalBoxOrigin(_attackOffset, _attackSize);
        //_animator.SetTrigger("Attack");

        OverlapBox(origin, _attackSize);

        for (int i = 0; i < HitBuffer.Count; i++)
        {
            Collider2D enemy = HitBuffer[i];
            if (!TryGetEnemyStat(enemy, out var enemyStat)) continue;

            enemyStat.TakeDamage(damage);

            if (enemy.TryGetComponent<IKnockbackable>(out var kb))
                kb.ApplyKnockback(facingDir, knockback);
        }

        IsCharging = false;
        _chargingTime = 0f;
        StartAttackCooldown();
    }

    public void ReleaseCharge(string actionName)
    {
        switch(actionName)
        {
            case "Attack": ReleaseAttack(); break;
        }
    }

    public override void Skill()
    {
        if(IsSkillOnCooldown()) return;
        _duration = 10f;
        _plusMoveSpeed = PlayerStat.MoveSpeed * _speedUpScale;
        PlayerStat.MoveSpeed += _plusMoveSpeed;// player의 속도 배율은 1 + _speedUpScale
        _isSpeedUp = true;
        StartSkillCooldown();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_attackOffset == null)
            _attackOffset = GetComponent<BoxCollider2D>();

        Vector2 origin = GetHorizontalBoxOrigin(_attackOffset, _attackSize);

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(origin, _attackSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, _attackSize);
    }
#endif
}
