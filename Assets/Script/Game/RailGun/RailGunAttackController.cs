using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStat))]
public class RailGunAttackController : MonoBehaviour, IAttackController
{
    [Header("공격 설정")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _attackLayerMask;
    [SerializeField] private Vector2 _attackSize = new Vector2(20f, 2f);

    [Header("차징 설정")]
    [SerializeField] private float _maxCharge = 3f;
    [SerializeField] private int[] _chargeDamageMultiplier = { 1, 2, 3, 4 }; // 0~3단계

    [Header("감지 설정")]
    [SerializeField] private float _triangleAngle = 45f;  // 차징 0%일때 각도
    [SerializeField] private float _detectionRange = 15f;

    [Header("Input")]
    [SerializeField] private InputActionReference _attackActionRef; // Inspector에서 Attack 액션 연결

    private PlayerController _playerController;
    private PlayerStat _playerStat;

    private float _chargeTimer = 0f;
    private bool _isCharging = false;
    private Transform _detectedTarget = null;
    [SerializeField] private GameObject _laserPrefab;

    List<Collider2D> _hitBuffer = new List<Collider2D>(15);
    private ContactFilter2D _contactFilter;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();

        _playerController.OnAttackHandler = Attack; // 기존 흐름 유지 (차징 시작)
        _playerController.OnSkillHandler = Skill;

        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_attackLayerMask);
        _contactFilter.useTriggers = true;
    }

    void OnEnable()
    {
        // 뗄 때(canceled)만 직접 구독 → 발사 처리
        _attackActionRef.action.canceled += OnAttackReleased;
    }

    void OnDisable()
    {
        _attackActionRef.action.canceled -= OnAttackReleased;
    }

    // PlayerController → 누를 때 호출됨 → 차징 시작
    public void Attack()
    {
        if (_playerStat.AttackCooltime > 0f) return;

        _isCharging = true;
        _chargeTimer = 0f;
    }

    // canceled 콜백 → 뗄 때 → 발사
    private void OnAttackReleased(InputAction.CallbackContext ctx)
    {
        if (!_isCharging) return;

        if (_playerController.IsGrounded)
            NormalAttack();
        else
            JumpAttack();

        _isCharging = false;
        _chargeTimer = 0f;
        _detectedTarget = null;

        _playerStat.AttackCooltime = _playerStat.AttackCooltimeMax;
    }

    void Update()
    {
        if (!_isCharging) return;

        _chargeTimer = Mathf.Clamp(_chargeTimer + Time.deltaTime, 0f, _maxCharge);
        _detectedTarget = DetectEnemy();
    }

    // ── 감지 ──────────────────────────────────
    Transform DetectEnemy()
    {
        float chargeRatio = _chargeTimer / _maxCharge;
        float currentAngle = Mathf.Lerp(_triangleAngle, 0f, chargeRatio); // 삼각형 → 직선
        Vector2 facingDir = GetFacingDirection();

        Physics2D.OverlapCircle(transform.position, _detectionRange, _contactFilter, _hitBuffer);

        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var col in _hitBuffer)
        {
            if (col.gameObject == gameObject) continue;
            if (!col.TryGetComponent<PlayerStat>(out var stat)) continue;
            if (stat.TeamId == _playerStat.TeamId) continue;

            Vector2 toEnemy = (col.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(facingDir, toEnemy);

            if (angle <= currentAngle / 2f)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col.transform;
                }
            }
        }

        return closest;
    }

    // ── 발사 ──────────────────────────────────
    public void NormalAttack()
    {
        int level = Mathf.Clamp(Mathf.FloorToInt(_chargeTimer), 1, _chargeDamageMultiplier.Length - 1);
        int damage = _playerStat.AttackDamage * _chargeDamageMultiplier[level];

        Vector2 fireDir = _detectedTarget != null
            ? (_detectedTarget.position - transform.position).normalized
            : GetFacingDirection();

        SpawnRailSprite(fireDir);

        if (_detectedTarget != null && _detectedTarget.TryGetComponent<PlayerStat>(out var enemyStat))
            if (enemyStat.TeamId != _playerStat.TeamId)
                enemyStat.TakeDamage(damage);

        Debug.Log($"[Railgun] Lv{level} | 데미지: {damage} | 방향: {fireDir}");
    }

    public void JumpAttack() { }

    public void Skill() { }

    // ── 스프라이트 생성 ────────────────────────
    void SpawnRailSprite(Vector2 direction)
    {
        // _attackPoint 없으면 자기 위치 기준
        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 스프라이트 중심이 origin에서 절반 길이만큼 앞에 오도록
        Vector2 spawnPos = origin + direction * (_attackSize.x / 2f);

        GameObject rail = new GameObject("Laser");
        rail.transform.position = spawnPos;
        rail.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        rail.transform.localScale = new Vector3(_attackSize.x, _attackSize.y, 1f);

        // 스프라이트 렌더러 자동 추가
        var sr = rail.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>()?.sprite; // 임시: 본인 스프라이트 사용
        // TODO: 레일건 전용 스프라이트 에셋 연결

        Destroy(rail, 0.15f);
    }

    // ── 유틸 ──────────────────────────────────
    Vector2 GetFacingDirection()
    {
        bool isFacingRight = transform.localScale.x > 0f;
        return isFacingRight ? Vector2.right : Vector2.left;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        float chargeRatio = _chargeTimer / _maxCharge;
        float currentAngle = Mathf.Lerp(_triangleAngle, 0f, chargeRatio);
        Vector2 facing = GetFacingDirection();

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Vector3 left = Quaternion.Euler(0, 0, currentAngle / 2f) * (Vector3)facing;
        Vector3 right = Quaternion.Euler(0, 0, -currentAngle / 2f) * (Vector3)facing;
        Gizmos.DrawRay(transform.position, left * _detectionRange);
        Gizmos.DrawRay(transform.position, right * _detectionRange);

        if (_detectedTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _detectedTarget.position);
        }
    }
#endif
}