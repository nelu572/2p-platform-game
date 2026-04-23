using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class WarriorAttackController : MonoBehaviour, IAttackController
{
    [Header("공격 설정")]
    [SerializeField] private int _attackDamage = 30;
    [SerializeField] private float _attackCooltimeMax = 0.8f;
    [SerializeField] private Vector2 _attackBoxSize = new Vector2(2f, 1.5f);
    private Transform _attackPoint;

    [Header("스킬 설정")]
    [SerializeField] private int _slashWaveDamage = 20;
    [SerializeField] private float _skillCooltimeMax = 3f;
    [SerializeField] private Vector2 _slashWaveBoxSize = new Vector2(3f, 2f);
    [SerializeField] private float _slashWaveOffset = 1f;    // 판정 중심 위치 (앞쪽으로 얼마나)
    [SerializeField] private float _knockbackForce = 8f;     // 맞은 상대 밀리는 힘
    [SerializeField] private float _selfKnockbackForce = 4f; // 자신의 반동 힘

    [Header("쿨타임")]
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }

    private PlayerController _playerController;
    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator animator;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //animator = GetComponent<Animator>();
        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;
    }

    void Update()
    {
        if (AttackCooltime > 0)
            AttackCooltime -= Time.deltaTime;
        if (SkillCooltime > 0)
            SkillCooltime -= Time.deltaTime;
    }

    public void Attack()
    {
        if (AttackCooltime > 0f)
            return;

        AttackCooltime = _attackCooltimeMax;
        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //if (_playerController._isGrounded)
        //    animator.SetTrigger("NormalAttack");
        //else
        //    animator.SetTrigger("JumpAttack");
        PerformAttackHit();
    }

    public void PerformAttackHit()
    {
        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position + new Vector2(isFacingRight ? 0.6f : -0.6f, 0f);

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(origin, _attackBoxSize, 0f);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject == gameObject) continue;

            if (enemy.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.TeamId != GetComponent<IDamageable>().TeamId)
                    damageable.TakeDamage(_attackDamage);
            }
        }
        //인터페이스에서 프로퍼티 값으로 적과 자신(아군)을 구별하고 있습니다
        //이 TeamId값은 InGameManager에서 직접 부여 해야합니다
        //다만 이건 의존성이 필요해서 따로 인터페이스를 분리해야 합니다
        //물론 2인용이 아니라 다인용일때 합니다 지금하면 오버엔지니어링에 걸릴 수 있습니다
    }

    public void Skill()
    {
        if (SkillCooltime > 0f)
            return;

        SkillCooltime = _skillCooltimeMax;
        PerformSlashWave();
    }

    private void PerformSlashWave()
    {
        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 skillDirection = isFacingRight ? Vector2.right : Vector2.left;

        // 검격 판정 위치 (바라보는 방향 앞쪽)
        Vector2 origin = (Vector2)transform.position + skillDirection * _slashWaveOffset;

        // 벽 무시 — 레이어 필터 없이 전체 탐지
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(origin, _slashWaveBoxSize, 0f);

        foreach (Collider2D col in hitColliders)
        {
            if (col.gameObject == gameObject) continue;

            if (col.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.TeamId != GetComponent<IDamageable>().TeamId)
                {
                    damageable.TakeDamage(_slashWaveDamage);

                    // 맞은 플레이어 밀기 (공격 방향으로)
                    if (col.TryGetComponent<Rigidbody2D>(out var rb))
                    {
                        rb.AddForce(skillDirection * _knockbackForce, ForceMode2D.Impulse);

                        // 상대 PlayerController에 knockback 상태 전달
                        if (col.TryGetComponent<PlayerController>(out var pc))
                            StartCoroutine(KnockbackRoutine(pc));
                    }
                }
            }
        }

        // 자신은 반동 (공격 반대 방향으로)
        if (TryGetComponent<Rigidbody2D>(out var selfRigidbody))
        {
            selfRigidbody.AddForce(-skillDirection * _selfKnockbackForce, ForceMode2D.Impulse);
            StartCoroutine(KnockbackRoutine(_playerController));
        }
    }
    private IEnumerator KnockbackRoutine(PlayerController pc)
    {
        pc.IsKnockedBack = true;

        Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
        while (rb.linearVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(rb.linearVelocity.x, 0f, Time.deltaTime * 5f),
                rb.linearVelocity.y  // y는 그대로 유지 (중력 영향 받도록)
            );
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        pc.IsKnockedBack = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 skillDirection = isFacingRight ? Vector2.right : Vector2.left;

        // 일반 공격 범위 (빨간색) — attackPoint 없으면 기본 위치로 표시
        Vector3 attackOrigin = _attackPoint != null
            ? _attackPoint.position
            : transform.position + new Vector3(isFacingRight ? 0.6f : -0.6f, 0f, 0f);

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(attackOrigin, _attackBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackOrigin, _attackBoxSize);

        // 스킬 범위 (파란색)
        Vector3 slashOrigin = transform.position + (Vector3)(skillDirection * _slashWaveOffset);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.35f);
        Gizmos.DrawCube(slashOrigin, _slashWaveBoxSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(slashOrigin, _slashWaveBoxSize);
    }
#endif
}