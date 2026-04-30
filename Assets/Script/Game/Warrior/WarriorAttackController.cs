using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WarriorAttackController : PlayerStat, IAttackController
{
    [Header("공격 설정")]
    //공격력
    [SerializeField] private float _attackCooltimeMax = 0.8f;
    [SerializeField] private Vector2 _attackBoxSize = new Vector2(2f, 1.5f);
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _attackLayerMask;

    [Header("스킬 설정 - 검기")]
    //공격력
    //스킬 쿨타임
    [SerializeField] private float _skillCooltimeMax = 10f;
    // 검기 이동 속도
    [SerializeField] private float _slashWaveSpeed = 10f;
    // 시전자 반동 힘
    [SerializeField] private float _selfKnockbackForce = 4f;
    // 검기 프리팹
    [SerializeField] private GameObject _slashWavePrefab;
    // 검기 최대 사거리
    [SerializeField] private float _slashWaveMaxDistance = 15f;
    // 검기 크기
    [SerializeField] private Vector2 _slashWaveScale = new Vector2(4.5f, 8f);

    [Header("쿨타임")]

    //델리게이트에 함수를 가져오기 위한 참조 변수
    private PlayerController _playerController;
    //자기 자신에 있는 인터페이스 캐싱용
    private IDamageable _selfDamageable;
    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator _animator;

    void Awake()
    {
        AttackDamage = 30;
        SkillDamage = 50;
        MaxHp = 100;
        Hp = MaxHp;

        _playerController = GetComponent<PlayerController>();
        _selfDamageable = this;
        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator = GetComponent<Animator>();
        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;

        _attackLayerMask += LayerMask.GetMask("Warrior");
        _attackLayerMask += LayerMask.GetMask("RailGun");
        _attackLayerMask += LayerMask.GetMask("Witch");
        _attackLayerMask += LayerMask.GetMask("BatMan");
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

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //if (_playerController.IsGrounded)
        //    _animator.SetTrigger("NormalAttack");
        //else
        //    _animator.SetTrigger("JumpAttack");
        PerformAttackHit();
        AttackCooltime = _attackCooltimeMax;
    }

    public void PerformAttackHit()
    {
        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position + new Vector2(isFacingRight ? 0.6f : -0.6f, 0f);

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(origin, _attackBoxSize, 0f, _attackLayerMask);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject == gameObject) continue;

            if (enemy.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.TeamId != _selfDamageable.TeamId)
                    damageable.TakeDamage(AttackDamage);
            }
        }
        //인터페이스에서 프로퍼티 값으로 적과 자신(아군)을 구별하고 있습니다
        ///TODO InGameManager에서 캐릭터에게 TeamId 부여하는 기능 필요
        //다만 이건 의존성이 필요해서 따로 인터페이스를 분리해야 합니다
        //물론 2인용이 아니라 다인용일때 합니다 지금하면 오버엔지니어링에 걸릴 수 있습니다
    }

    public void Skill()
    {
        if (SkillCooltime > 0f)
            return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator.SetTrigger("Skill");

        FireSlashWave();
        SkillCooltime = _skillCooltimeMax;
    }

    private void FireSlashWave()
    {
        if (_slashWavePrefab == null)
        {
            Debug.LogWarning("SlashWave 프리팹이 없습니다!");
            return;
        }

        bool isFacingRight = transform.localScale.x > 0f;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

        // 검기 생성
        GameObject slashWaveObj = Instantiate(
            _slashWavePrefab,
            transform.position,
            Quaternion.identity

        );

        // 방향에 따라 스프라이트 반전
        Vector3 waveScale = new Vector3(_slashWaveScale.x * (isFacingRight ? 1f : -1f), _slashWaveScale.y, 1f);
        slashWaveObj.transform.localScale = waveScale;

        // 검기 초기화
        if (slashWaveObj.TryGetComponent<SlashWave>(out var slashWave))
            slashWave.Initialize(SkillDamage, _selfDamageable.TeamId, direction, _slashWaveSpeed, _slashWaveMaxDistance, _attackLayerMask);

        // 시전자 반동
        _playerController.ApplyKnockback(-direction, _selfKnockbackForce);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        bool isFacingRight = transform.localScale.x > 0f;

        // 일반 공격 범위 (빨간색)
        Vector3 attackOrigin = _attackPoint != null
            ? _attackPoint.position
            : transform.position + new Vector3(isFacingRight ? 0.6f : -0.6f, 0f, 0f);

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(attackOrigin, _attackBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackOrigin, _attackBoxSize);

    }
#endif
}