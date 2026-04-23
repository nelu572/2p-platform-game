using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WarriorAttackController : MonoBehaviour, IAttackController
{
    [Header("공격 설정")]
    [SerializeField] private int _attackDamage = 30;
    [SerializeField] private float _attackCooltimeMax = 0.8f;
    [SerializeField] private Vector2 _attackBoxSize = new Vector2(1.2f, 0.8f);
    private LayerMask _enemyLayer;
    private Transform _attackPoint;
    [Header("쿨타임")]
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }

    private PlayerController _playerController;

    // 에니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator animator;

    void Awake()
    {
        
        // 에니메이션이 생긴다면 주석처리를 해제할 것입니다
        //animator = GetComponent<Animator>();
        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;

        //임의로 만든 레이어 입니다 추후 변경될 수 있습니다
        _enemyLayer = LayerMask.GetMask("Enemy");
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
        bool isFacingRight = transform.localScale.x > 0f;  // localScale로 방향 판단

        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position + new Vector2(isFacingRight ? 0.6f : -0.6f, 0f);

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(origin, _attackBoxSize, 0f, _enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<IDamageable>(out var damageable)
                 && enemy.gameObject == gameObject)
                damageable.TakeDamage(_attackDamage);
        }
    }

    public void Skill()
    {
        // 추후 구현
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