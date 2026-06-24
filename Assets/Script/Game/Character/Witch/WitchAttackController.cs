using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ChargeInputHandler))]
public class WitchAttackController : BaseAttackController, IChargeable
{
    [Header("일반 공격")]
    [SerializeField] private float _throwPower = 1f;
    [SerializeField] private float _throwMaxPower = 30f;
    [SerializeField] private float _throwChargeSpeed = 7.5f; // 초당 충전되는 힘의 양
    [SerializeField] private float _throwAngle = 30f; // 던지는 각도
    [SerializeField] private float _throwSpawnOffset = 1.2f;
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

    [Header("범위 표시")]
    [SerializeField] private float _trajectoryGravityScale = 1f;

    private ObjectPoolManager _objectPoolManager;
    //private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        //Awake에서 ObjectPoolManager가 싱글톤이 되기에 Start에서 참조하는 것이 안전합니다.
        _objectPoolManager = ObjectPoolManager.Instance;
    }

    void Update()
    {
        if (IsCharging)
        {
            _throwPower += Time.deltaTime * _throwChargeSpeed;
            _throwPower = Mathf.Min(_throwPower, _throwMaxPower);
            ShowThrowTrajectory();
        }
    }

    public override void Attack()
    {
        if(IsAttackOnCooldown())
            return;

        if (PlayerController.IsGrounded)
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
        GetChildVisibleAttack<WitchVisibleAttack>("WitchVisibleAttack")?.Hide();

        // 포션 꺼내기
        _potion = _objectPoolManager.Get(_potions[_potionIndex]);
        if (_potion == null) return;

        // 포션 초기 위치를 플레이어 위치로
        _potion.transform.position = transform.position;

        Vector2 throwDir = GetThrowDirection();

        // 플레이어 콜라이더 크기만큼 앞에서 생성
        _potion.transform.position = GetThrowStartPosition(throwDir);

        // Rigidbody2D에 힘 주입
        if (_potion.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero; // 이전 속도 초기화
            rb.AddForce(throwDir * _throwPower, ForceMode2D.Impulse);
        }

        // 포션에 스탯 전달
        if (_potion.TryGetComponent<Potion>(out var potion))
            potion.Initialize(PlayerStat, _potionCollisionLayerMask);

        Debug.Log($"[Witch] {potion.name}물약 던짐 힘: {_throwPower}");
        _throwPower = 0f;
        StartAttackCooldown();

    }

    public void JumpAttack()
    {
        GetChildVisibleAttack<WitchVisibleAttack>("WitchVisibleAttack")?.Hide();

        GameObject obj = _objectPoolManager.Get(_windPotionKey);
        if (obj == null) return;

        float offsetY = BodyCollider.size.y * 0.5f;
        obj.transform.position = (Vector2)transform.position + Vector2.down * offsetY;

        if (obj.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.down * _throwMaxPower, ForceMode2D.Impulse);
        }

        if (obj.TryGetComponent<Potion>(out var potion))
            potion.Initialize(PlayerStat, _potionCollisionLayerMask);

        StartAttackCooldown();
    }

    // 스킬 - 포션 순환 선택
    public override void Skill()
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

    private void ShowThrowTrajectory()
    {
        Vector2 throwDir = GetThrowDirection();
        Vector2 startPosition = GetThrowStartPosition(throwDir);
        Vector2 initialVelocity = throwDir * _throwPower;
        GetChildVisibleAttack<WitchVisibleAttack>("WitchVisibleAttack")?.ShowThrowTrajectory(startPosition, initialVelocity, _trajectoryGravityScale);
    }

    private Vector2 GetThrowDirection()
    {
        Vector2 facingDir = GetFacingDirection();
        float rad = _throwAngle * Mathf.Deg2Rad;
        return new Vector2(
            facingDir.x * Mathf.Cos(rad),
            Mathf.Sin(rad)
        );
    }

    private Vector2 GetThrowStartPosition(Vector2 throwDir)
    {
        return (Vector2)transform.position + throwDir * _throwSpawnOffset;
    }
}
