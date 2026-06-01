using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ChargeInputHandler))]
public class BatManAttackController : MonoBehaviour, IAttackController, IChargeable
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

    private PlayerController _playerController;
    private PlayerStat _playerStat;
    //private Animator _animator;

    List<Collider2D> _hitBuffer = new List<Collider2D>(15);
    private ContactFilter2D _contactFilter;
    public bool IsCharging { get; set; }

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();
        _attackOffset = GetComponent<BoxCollider2D>();
        //_animator = GetComponent<Animator>();

        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;

        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_attackLayerMask);
        _contactFilter.useTriggers = true;
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
                _playerStat.MoveSpeed -= _plusMoveSpeed;
            }
        }
    }

    public void Attack()
    {
        if (IsCharging) return;
        if (_playerStat.AttackCooltime > 0f) return; // 추가

        IsCharging = true;
        //_animator.SetTrigger("Chargeing");
    }

    public void ReleaseAttack()
    {
        if (!IsCharging) return;
        
        int level = Mathf.Clamp(Mathf.FloorToInt(_chargingTime), 0, _chargeMultiplier.Length - 1);
        int damage = _playerStat.AttackDamage * _chargeMultiplier[level];
        float knockback = _knockbackPower * _chargeMultiplier[level];

        Vector2 facingDir = transform.localScale.x > 0f ? Vector2.right : Vector2.left;
        float offsetX = (_attackOffset.offset.x + _attackOffset.size.x / 2f + _attackSize.x / 2f) * facingDir.x;
        Vector2 origin = (Vector2)transform.position + new Vector2(offsetX, 0f);
        //_animator.SetTrigger("Attack");

        _hitBuffer.Clear();
        Physics2D.OverlapBox(origin, _attackSize, 0f, _contactFilter, _hitBuffer);

        for (int i = 0; i < _hitBuffer.Count; i++)
        {
            Collider2D enemy = _hitBuffer[i];
            if (enemy.gameObject == gameObject) continue;

            if (enemy.TryGetComponent<PlayerStat>(out var enemyStat))
            {
                if (enemyStat.TeamId != _playerStat.TeamId)
                    enemyStat.TakeDamage(damage);
            }

            if (enemy.TryGetComponent<IKnockbackable>(out var kb))
                kb.ApplyKnockback(facingDir, knockback);
        }

        IsCharging = false;
        _chargingTime = 0f; 
        _playerStat.AttackCooltime = _playerStat.AttackCooltimeMax;
    }

    public void ReleaseCharge(string actionName)
    {
        switch(actionName)
        {
            case "Attack": ReleaseAttack(); break;
        }
    }

    public void Skill()
    {
        if(_playerStat.SkillCooltime > 0f) return;
        _duration = 10f;
        _plusMoveSpeed = _playerStat.MoveSpeed * _speedUpScale;
        _playerStat.MoveSpeed += _plusMoveSpeed;// player의 속도 배율은 1 + _speedUpScale
        _isSpeedUp = true;
        _playerStat.SkillCooltime = _playerStat.SkillCooltimeMax;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 facingDir = transform.localScale.x > 0f ? Vector2.right : Vector2.left;
        float offsetX = (_attackOffset.offset.x + _attackOffset.size.x / 2f + _attackSize.x / 2f) * facingDir.x;
        Vector2 origin = (Vector2)transform.position + new Vector2(offsetX, 0f);

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(origin, _attackSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, _attackSize);
    }
#endif
}
