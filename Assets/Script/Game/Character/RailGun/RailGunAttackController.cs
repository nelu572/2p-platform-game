using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ChargeInputHandler))]
public class RailGunAttackController : BaseAttackController, IChargeable
{
    [Header("일반 공격")]
    //공격 위치(인스펙터에서 만지지 말아야 제대로 된 공격 위치가됨)
    [SerializeField] private Transform _attackPoint;
    //감지할 대상의 레이어
    [SerializeField] private LayerMask _attackLayerMask;
    //공격 크기
    [SerializeField] private Vector2 _attackSize = new Vector2(20f, 2f);
    // 차징 0%일때 감지 범위 y값
    [SerializeField] private float _maxWidth = 8f;
    [SerializeField] private float _maxCharge = 3f;
    [SerializeField] private int[] _chargeDamageMultiplier = { 1, 2, 3, 5 }; // 0~3단계

    [Header("스킬")]
    // 스킬은 일반 공격의 2배
    [SerializeField] private Vector2 _skillAttackSize = new Vector2(40f, 4f);
    [SerializeField] private float _skillMaxWidth = 16f;
    [SerializeField] private float _skillMaxCharge = 6f;

    [Header("레이저")]
    [SerializeField] private float _laserDuration = 0.5f;
    // 오브젝트 폴링에 넘길 key값
    // 이 오브젝트로 생성할 오브젝트를 찾은후 오브젝트 활성화
    [SerializeField] private string _laserKey = "Laser";

    [Header("넉백")]
    [SerializeField] private float _jumpKnockbackPower = 16f;
    [SerializeField] private float _playerKnockbackPower = 2f;
    [SerializeField] private float _enemyKnockbackPower = 3f;

    //레이저 생성 클래스
    private ObjectPoolManager _objectPoolManager;
    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator _animator;

    private float _chargeTimer = 0f;
    public bool IsCharging { get; set; }
    private Transform _detectedTarget = null;

    private float _skillChargeTimer = 0f;
    private bool _isSkillCharging = false;
    private Transform _skillDetectedTarget = null;

    protected override void Awake()
    {
        base.Awake();
        SetupContactFilter(_attackLayerMask);
    }

    void Start()
    {
        //Awake에서 ObjectPoolManager가 싱글톤이 되기에 Start에서 참조하는 것이 안전합니다.
        _objectPoolManager = ObjectPoolManager.Instance;
    }

    void Update()
    {

        // 일반 차징
        if (IsCharging)
        {
            _chargeTimer = Mathf.Clamp(_chargeTimer + Time.deltaTime, 0f, _maxCharge);
            _detectedTarget = DetectEnemy(_attackSize.x, _maxWidth, _attackSize.y, _maxCharge, _chargeTimer);
        }

        // 스킬 차징
        if (_isSkillCharging)
        {
            _skillChargeTimer = Mathf.Clamp(_skillChargeTimer + Time.deltaTime, 0f, _skillMaxCharge);
            _skillDetectedTarget = DetectEnemy(_skillAttackSize.x, _skillMaxWidth, _skillAttackSize.y, _skillMaxCharge, _skillChargeTimer);
        }
    }

    // PlayerController → 누를 때 호출됨 → 차징 시작
    public override void Attack()
    {
        if (IsAttackOnCooldown()) return;

        //if (PlayerController.IsGrounded)
        //_animator.SetTrigger("NormalAttack");
        //else
        //_animator.SetTrigger("JumpAttack");

        IsCharging = true;
        _chargeTimer = 0f;
    }

    private void ReleaseAttack()
    {   //공격키 해제시 호출됨
        if (!IsCharging) return;

        if (PlayerController.IsGrounded)
            NormalAttack();
        else
            JumpAttack();

        IsCharging = false;
        _chargeTimer = 0f;
        _detectedTarget = null;
        StartAttackCooldown();
    }

    public void NormalAttack()
    {
        int level = Mathf.Clamp(Mathf.FloorToInt(_chargeTimer), 0, _chargeDamageMultiplier.Length - 1);
        int damage = PlayerStat.AttackDamage * _chargeDamageMultiplier[level];

        Vector2 fireDir = _detectedTarget != null
            ? (_detectedTarget.position - transform.position).normalized
            : GetFacingDirection();

        SpawnRailSprite(fireDir, _attackSize.x, _attackSize.y);

        if (_detectedTarget != null && _detectedTarget.TryGetComponent<PlayerStat>(out var enemyStat))
        {
            if (enemyStat.TeamId != PlayerStat.TeamId)
            {
                enemyStat.TakeDamage(damage);
                if (_detectedTarget.TryGetComponent<IKnockbackable>(out var pc))
                    pc.ApplyKnockback(fireDir, _enemyKnockbackPower * (level + 1f));
            }
        }

        PlayerController.ApplyKnockback(-fireDir, _playerKnockbackPower * level + 1f); // 차징 시간에 비례한 넉백
        Debug.Log($"[Railgun] Lv{level} | 데미지: {damage} | 방향: {fireDir}");
    }

    public void JumpAttack()
    {
        int level = Mathf.Clamp(Mathf.FloorToInt(_chargeTimer), 0, _chargeDamageMultiplier.Length - 1);
        int damage = PlayerStat.AttackDamage * _chargeDamageMultiplier[level];

        // 무조건 아래 방향
        Vector2 fireDir = Vector2.down;

        SpawnRailSprite(fireDir, _attackSize.x, _attackSize.y);

        // 아래 방향 감지 (점프공격은 별도 감지 없이 바로 아래 OverlapBox)
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (_attackSize.x / 2f);
        OverlapBox(boxCenter, new Vector2(_attackSize.y, _attackSize.x));
        // attackSize를 90도 회전 → x,y 반전 (세로로 긴 박스)

        foreach (var col in HitBuffer)
        {
            if (!TryGetEnemyStat(col, out var enemyStat)) continue;

            enemyStat.TakeDamage(damage);
            if (col.TryGetComponent<IKnockbackable>(out var kb))
                kb.ApplyKnockback(fireDir, _enemyKnockbackPower * (level + 1f));
        }
        //속도 초기화
        PlayerController.ResetFallSpeed();
        // 차징 시간에 비례한 위쪽 반동
        PlayerController.ApplyKnockback(Vector2.up, _jumpKnockbackPower * level + 1f);

    }

    public override void Skill()
    {
        if (IsSkillOnCooldown()) return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator.SetTrigger("Skill");

        _isSkillCharging = true;
        _skillChargeTimer = 0f;
    }

    private void ReleaseSkill()
    {   //스킬키 해제시 호출됨
        if (!_isSkillCharging) return;

        FireSkill();

        _isSkillCharging = false;
        _skillChargeTimer = 0f;
        _skillDetectedTarget = null;
        StartSkillCooldown();
    }

    private void FireSkill()
    {
        int level = Mathf.Clamp(Mathf.FloorToInt(_skillChargeTimer), 0, _chargeDamageMultiplier.Length - 1);
        int damage = PlayerStat.SkillDamage * _chargeDamageMultiplier[level];

        Vector2 fireDir = _skillDetectedTarget != null
            ? (_skillDetectedTarget.position - transform.position).normalized
            : GetFacingDirection();

        SpawnRailSprite(fireDir, _skillAttackSize.x, _skillAttackSize.y);

        if (_skillDetectedTarget != null && _skillDetectedTarget.TryGetComponent<PlayerStat>(out var enemyStat))
        {
            if (enemyStat.TeamId != PlayerStat.TeamId)
            {
                enemyStat.TakeDamage(damage);
                if (_skillDetectedTarget.TryGetComponent<IKnockbackable>(out var kb))
                    kb.ApplyKnockback(fireDir, _enemyKnockbackPower * (level + 1f) * 2f); // 넉백도 2배
            }
        }

        PlayerController.ApplyKnockback(-fireDir, (_playerKnockbackPower * level + 1f) * 2f); // 반동도 2배
        Debug.Log($"[Railgun Skill] Lv{level} | 데미지: {damage} | 방향: {fireDir}");
    }

    public void ReleaseCharge(string actionName)
    {//어떤 공격 액션키를 사용했는지 확인하고 그에 맞는 함수를 호출
        switch (actionName)
        {
            case "Attack": ReleaseAttack(); break;
            case "Skill":  ReleaseSkill();  break;
        }
    }

    private Transform DetectEnemy(float sizeX, float maxWidth, float minSizeY, float maxCharge, float chargeTimer)
    {
        float chargeRatio = chargeTimer / maxCharge;
        float attackSizeY = Mathf.Lerp(maxWidth, minSizeY, chargeRatio); // 감지 범위 y값 축소
        Vector2 facingDir = GetFacingDirection();

        // 박스 중심을 바라보는 방향으로 오프셋
        Vector2 boxCenter = (Vector2)transform.position + facingDir * (sizeX / 2f);
        OverlapBox(boxCenter, new Vector2(sizeX, attackSizeY));

        Transform closest = null;
        //가장 가까운 적의 거리
        float closestDist = Mathf.Infinity;

        foreach (var col in HitBuffer)
        {
            if (!TryGetEnemyStat(col, out _)) continue;

            float distSqr = ((Vector2)transform.position - (Vector2)col.transform.position).sqrMagnitude;
            if (distSqr < closestDist)
            {
                closestDist = distSqr;
                closest = col.transform;
            }
        }

        return closest;
    }

    private void SpawnRailSprite(Vector2 direction, float sizeX, float sizeY)
    {
        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector2 spawnPos = origin + direction * (sizeX / 2f);

        GameObject laser = _objectPoolManager.Get(_laserKey);
        if (laser == null)
            return;

        laser.transform.position = spawnPos;
        laser.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        laser.transform.localScale = new Vector3(sizeX, sizeY, 1f);
        // SetActive 중복 제거 → Get() 내부에서 처리

        if (laser.TryGetComponent<SelfReturn>(out var selfReturn))
            selfReturn.ReturnAfter(_laserDuration);
        else
            Debug.LogWarning("Laser 프리팹에 SelfReturn 컴포넌트가 없습니다.", laser);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 facing = GetFacingDirection();

        // 일반 공격 감지 범위 (노란색)
        if (IsCharging)
        {
            float chargeRatio = _chargeTimer / _maxCharge;
            float attackSizeY = Mathf.Lerp(_maxWidth, _attackSize.y, chargeRatio); // 감지 범위 y값 축소

            Vector3 boxCenter = transform.position + (Vector3)(facing * (_attackSize.x / 2f));
            Vector3 boxSize = new Vector3(_attackSize.x, attackSizeY, 0f);

            Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
            Gizmos.DrawCube(boxCenter, boxSize);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boxCenter, boxSize);

            // 감지된 적 (빨간선)
            if (_detectedTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _detectedTarget.position);
            }
        }

        // 스킬 감지 범위 (파란색)
        if (_isSkillCharging)
        {
            float skillChargeRatio = _skillChargeTimer / _skillMaxCharge;
            float skillSizeY = Mathf.Lerp(_skillMaxWidth, _skillAttackSize.y, skillChargeRatio); // 감지 범위 y값 축소

            Vector3 skillBoxCenter = transform.position + (Vector3)(facing * (_skillAttackSize.x / 2f));
            Vector3 skillBoxSize = new Vector3(_skillAttackSize.x, skillSizeY, 0f);

            Gizmos.color = new Color(0f, 0.5f, 1f, 0.35f);
            Gizmos.DrawCube(skillBoxCenter, skillBoxSize);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(skillBoxCenter, skillBoxSize);

            // 감지된 적 (주황선)
            if (_skillDetectedTarget != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f);
                Gizmos.DrawLine(transform.position, _skillDetectedTarget.position);
            }
        }
    }
#endif
}
