using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class WarriorAttackController : BaseAttackController
{
    [Header("공격 설정")]
    [SerializeField] private Vector2 _attackBoxSize = new Vector2(2f, 1.5f);
    [SerializeField] private Transform _attackPoint;
    //감지 레이어
    [SerializeField] private LayerMask _attackLayerMask;

    [Header("스킬 설정 - 검기")]
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

    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator _animator;
    private BoxCollider2D _attackOffset;

    protected override void Awake()
    {
        base.Awake();
        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator = GetComponent<Animator>();
        _attackOffset = BodyCollider;

        // LayerMask를 ContactFilter2D로 변환
        SetupContactFilter(_attackLayerMask);
    }

    public override void Attack()
    {
        if (IsAttackOnCooldown())
            return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //if (PlayerController.IsGrounded)
        //    _animator.SetTrigger("NormalAttack");
        //else
        //    _animator.SetTrigger("JumpAttack");
        PerformAttackHit();
        StartAttackCooldown();
    }

    public void PerformAttackHit()
    {
        Vector2 origin = GetHorizontalBoxOrigin(_attackOffset, _attackBoxSize);

        OverlapBox(origin, _attackBoxSize);

        for (int i = 0; i < HitBuffer.Count; i++)
        {
            Collider2D enemy = HitBuffer[i];
            if (TryGetEnemyStat(enemy, out var enemyStat))
                enemyStat.TakeDamage(PlayerStat.AttackDamage);
        }
        //인터페이스에서 프로퍼티 값으로 적과 자신(아군)을 구별하고 있습니다
        ///TODO InGameManager에서 캐릭터에게 TeamId 부여하는 기능 필요
        //다만 이건 의존성이 필요해서 따로 인터페이스를 분리해야 합니다
        //물론 2인용이 아니라 다인용일때 합니다 지금하면 오버엔지니어링에 걸릴 수 있습니다
    }

    public override void Skill()
    {
        if (IsSkillOnCooldown())
            return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator.SetTrigger("Skill");

        FireSlashWave();
        StartSkillCooldown();
    }

    private void FireSlashWave()
    {
        if (_slashWavePrefab == null)
        {
            Debug.LogWarning("SlashWave 프리팹이 없습니다!");
            return;
        }

        bool isFacingRight = IsFacingRight();
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
            slashWave.Initialize(PlayerStat.SkillDamage, PlayerStat.TeamId, direction, _slashWaveSpeed, _slashWaveMaxDistance, _attackLayerMask);

        // 시전자 반동
        PlayerController.ApplyKnockback(-direction, _selfKnockbackForce);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_attackOffset == null)
            _attackOffset = GetComponent<BoxCollider2D>();

        Vector2 origin = GetHorizontalBoxOrigin(_attackOffset, _attackBoxSize);
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawCube(origin, _attackBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, _attackBoxSize);

    }
#endif
}
