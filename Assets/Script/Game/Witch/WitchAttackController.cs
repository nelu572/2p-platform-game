using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(ChargeInputHandler))]
public class WitchAttackController : MonoBehaviour, IAttackController, IChargeable
{
    [Header("일반 공격")]
    [SerializeField] private float _throwPower = 1f;
    [SerializeField] private float _throwMaxPower = 30f;
    [SerializeField] private float _throwChargeSpeed = 7.5f; // 초당 충전되는 힘의 양
    [SerializeField] private float _throwAngle = 30f; // 던지는 각도
    [SerializeField] private string[] _potions = { "Pain", "Poison", "Slow" };
    //캐릭터 레이어 + 땅 레이어를 가짐
    [SerializeField] private LayerMask _potionCollisionLayerMask;
    //이 오브젝트는 오브젝트 풀에서 꺼내오기 떄문에 인스펙터에 보일 필요가 없습니다.
    private GameObject _potion;
    public bool IsCharging { get; set; }

    [Header("점프 공격")]
    [SerializeField] private string _windPotionKey = "Wind"; // 바람 포션 전용 키

    [Header("스킬 - 포션 선택")]
    [SerializeField] private int _potionIndex = 0;

    private PlayerController _playerController;
    private PlayerStat _playerStat;
    private ObjectPoolManager _objectPoolManager;
    //private Animator _animator;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStat = GetComponent<PlayerStat>();
        _objectPoolManager = GameObject.Find("PoolManager").GetComponent<ObjectPoolManager>();

        _playerController.OnAttackHandler = Attack;
        _playerController.OnSkillHandler = Skill;
    }

    void Update()
    {
        if (IsCharging)
        {
            _throwPower += Time.deltaTime * _throwChargeSpeed;
            _throwPower = Mathf.Min(_throwPower, _throwMaxPower);
        }
    }

    public void Attack()
    {
        if(_playerStat.AttackCooltime > 0f)
            return;

        if (_playerController.IsGrounded)
        {
            //_animator.SetTrigger("ReadyThrow");
            IsCharging = true;
            Debug.Log("[Witch] 공격 준비 시작");
        }
        else
        {
            //_animator.SetTrigger("JumpAttack");
            JumpAttack();
        }
    }

    public void ReleaseAttack()
    {
        //_animator.SetTrigger("Throw");
        if (!IsCharging) return;

        IsCharging = false;

        // 포션 꺼내기
        _potion = _objectPoolManager.Get(_potions[_potionIndex]);
        if (_potion == null) return;

        // 포션 초기 위치를 플레이어 위치로
        _potion.transform.position = transform.position;

        // 바라보는 방향
        Vector2 facingDir = _playerController.transform.localScale.x > 0f
            ? Vector2.right
            : Vector2.left;

        // 포물선 방향 계산 (바라보는 방향 + 위쪽 각도)
        float rad = _throwAngle * Mathf.Deg2Rad;

        Vector2 throwDir = new Vector2(
            facingDir.x * Mathf.Cos(rad),
            Mathf.Sin(rad)
        );

        // 플레이어 콜라이더 크기만큼 앞에서 생성
        _potion.transform.position = (Vector2)transform.position + throwDir * 1.2f; // 1f는 오프셋, 조절 가능

        // Rigidbody2D에 힘 주입
        if (_potion.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero; // 이전 속도 초기화
            rb.AddForce(throwDir * _throwPower, ForceMode2D.Impulse);
        }

        // 포션에 스탯 전달
        if (_potion.TryGetComponent<Potion>(out var potion))
            potion.Initialize(_playerStat, _potionCollisionLayerMask);

        Debug.Log($"[Witch] {potion.name}물약 던짐 힘: {_throwPower}");
        _throwPower = 0f;
        _playerStat.AttackCooltime = _playerStat.AttackCooltimeMax;

    }

    public void JumpAttack()
    {
        GameObject obj = _objectPoolManager.Get(_windPotionKey);
        if (obj == null) return;

        float offsetY = GetComponent<BoxCollider2D>().size.y * 0.5f;
        obj.transform.position = (Vector2)transform.position + Vector2.down * offsetY;

        if (obj.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.down * _throwMaxPower, ForceMode2D.Impulse);
        }

        if (obj.TryGetComponent<Potion>(out var potion))
            potion.Initialize(_playerStat, _potionCollisionLayerMask);

        _playerStat.AttackCooltime = _playerStat.AttackCooltimeMax;
    }

    // 스킬 - 포션 순환 선택
    public void Skill()
    {
        _potionIndex = (_potionIndex + 1) % _potions.Length; // 끝에 도달하면 0으로 순환
        Debug.Log($"[Witch] 선택된 포션: {_potions[_potionIndex]}");
    }

    public void ReleaseCharge(string actionName)
    {
        switch (actionName)
        {
            case "Attack": ReleaseAttack(); break;
        }
    }
}