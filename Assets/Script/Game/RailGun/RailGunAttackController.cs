using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(ChargeInputHandler))]
public class RailGunAttackController : MonoBehaviour, IAttackController, IChargeable
{
    [Header("공격")]
    //공격 위치(인스펙터에서 만지지 말아야 제대로 된 공격 위치가됨)
    [SerializeField] private Transform _attackPoint;
    //감지할 대상의 레이어
    [SerializeField] private LayerMask _attackLayerMask;
    //공격 크기
    [SerializeField] private Vector2 _attackSize = new Vector2(20f, 2f);

    [Header("차징 설정")]
    [SerializeField] private float _maxCharge = 3f;
    [SerializeField] private int[] _chargeDamageMultiplier = { 1, 2, 3, 4 }; // 0~3단계

    [Header("감지 설정")]
    [SerializeField] private float _maxWidth = 8f;  // 차징 0%일때 감지 범위 y값
    //레이저 오브젝트
    [SerializeField] private GameObject _laserPrefab;

    //델리게이트에 함수를 가져오기 위한 참조 변수
    private PlayerController _playerController;
    //쿨타임 공격력 가져오는 참조 변수
    private PlayerStat _playerStat;
    // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
    //private Animator _animator;

    private float _chargeTimer = 0f;
    private bool _isCharging = false;
    private Transform _detectedTarget = null;

    //GC부하를 줄이기 위해 미리 Collider2D<>버퍼 생성
    //리스트도 동적 배열이라서 크기가 변경되면 재할당이 발생하지만
    //이미 존재하는 리스트를 사용하기에 OverlapBoxAll보다는 성능면에서는 좋다
    List<Collider2D> _hitBuffer = new List<Collider2D>(15);
    private ContactFilter2D _contactFilter;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();

        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;

        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_attackLayerMask);
        _contactFilter.useTriggers = true;
    }

    // PlayerController → 누를 때 호출됨 → 차징 시작
    public void Attack()
    {
        if (_playerStat.AttackCooltime > 0f) return;

        // 애니메이션이 생긴다면 주석처리를 해제할 것입니다
        //_animator.SetTrigger("NormalAttack");

        _isCharging = true;
        _chargeTimer = 0f;
    }

    public void ReleaseAttack()
    {//ChargeInputController에서 제어
        Debug.Log($"[Railgun] ReleaseAttack 호출됨"); // 확인용
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
        //차징 상태일때만 작동
        _chargeTimer = Mathf.Clamp(_chargeTimer + Time.deltaTime, 0f, _maxCharge);
        _detectedTarget = DetectEnemy();
    }

    private Transform DetectEnemy()
    {
        float chargeRatio = _chargeTimer / _maxCharge;
        float attackSizeY = Mathf.Lerp(_maxWidth, _attackSize.y, chargeRatio); // 감지 범위 y값 축소
        Vector2 facingDir = GetFacingDirection();

        // 박스 중심을 바라보는 방향으로 오프셋
        Vector2 boxCenter = (Vector2)transform.position + facingDir * (_attackSize.x / 2f);
        Physics2D.OverlapBox(boxCenter, new Vector2(_attackSize.x, attackSizeY), 0f, _contactFilter, _hitBuffer);

        Transform closest = null;
        float closestDist = Mathf.Infinity; //가장 가까운 적의 거리

        foreach (var col in _hitBuffer)
        {
            if (col.gameObject == gameObject) continue; // 자기 자신은 제외
            if (!col.TryGetComponent<PlayerStat>(out var stat)) continue; //PlayerStat없을 시 제외
            if (stat.TeamId == _playerStat.TeamId) continue; //같은 팀 제외

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = col.transform;
            }
        }

        return closest;
    }

    public void NormalAttack()
    {
        int level = Mathf.Clamp(Mathf.FloorToInt(_chargeTimer), 0, _chargeDamageMultiplier.Length - 1);
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

    private void SpawnRailSprite(Vector2 direction)
    {
        // _attackPoint 없으면 자기 위치 기준
        Vector2 origin = _attackPoint != null
            ? (Vector2)_attackPoint.position
            : (Vector2)transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 스프라이트 중심이 origin에서 절반 길이만큼 앞에 오도록
        Vector2 spawnPos = origin + direction * (_attackSize.x / 2f);

        GameObject rail = Instantiate(_laserPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        //크기 설정
        rail.transform.localScale = new Vector3(_attackSize.x, _attackSize.y, 1f);

        Destroy(rail, 0.5f);
    }

    Vector2 GetFacingDirection()
    {
        return transform.localScale.x > 0f ? Vector2.right : Vector2.left;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        float chargeRatio = _chargeTimer / _maxCharge;
        float attackSizeY = Mathf.Lerp(_maxWidth, _attackSize.y, chargeRatio); // 감지 범위 y값 축소
        Vector2 facing = GetFacingDirection();

        // 박스 중심을 바라보는 방향으로 오프셋
        Vector3 boxCenter = transform.position + (Vector3)(facing * (_attackSize.x / 2f));
        Vector3 boxSize = new Vector3(_attackSize.x, attackSizeY, 0f);

        // 감지 범위 박스 (노란색)
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
#endif
}