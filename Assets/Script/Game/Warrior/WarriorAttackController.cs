using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WarriorAttackController : MonoBehaviour, IAttackController
{
    [Header("공격 설정")]
    [SerializeField] private int _attackDamage = 30;
    [SerializeField] private float _attackCooltimeMax = 0.8f;
    [SerializeField] private Vector2 _attackBoxSize = new Vector2(1.2f, 0.8f);
    private Transform _attackPoint;
    [Header("쿨타임")]
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }

    private PlayerController _playerController;

    // 에니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator animator;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        // 에니메이션이 생긴다면 주석처리를 해제할 것입니다
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
        // 쿨타임 초기화
        AttackCooltime = _attackCooltimeMax;

        // 에니메이션이 생긴다면 주석처리를 해제할 것입니다
        //animator.SetTrigger("Attack");

        // 공격 판정
        PerformAttackHit();
    }

    public void PerformAttackHit()
    {
        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position + new Vector2(isFacingRight ? 0.6f : -0.6f, 0f);

        // 레이어 필터 없이 전체 탐지
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(origin, _attackBoxSize, 0f);

        foreach (Collider2D enemy in hitEnemies)
        {
            // 자기 자신 제외
            if (enemy.gameObject == gameObject) continue;

            if (enemy.TryGetComponent<IDamageable>(out var damageable))
            {
                // 다른 팀만 데미지
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
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(_attackPoint.position, _attackBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_attackPoint.position, _attackBoxSize);
    }
#endif
}