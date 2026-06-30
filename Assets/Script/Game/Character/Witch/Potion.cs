using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PooledObject))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class Potion : MonoBehaviour
{
    [SerializeField] private PooledObject _pooled;
    //데미지는 포션이 넣기 떄문에 PlayerStat이 여기도 와아합니다
    [SerializeField] protected PlayerStat _playerStat;

    [Header("공격 감지")]
    // 포션의 영역을 자식에서 조절하도록 protected로 설정
    [SerializeField] protected Vector2 _overlapSize = new Vector2(8f, 1f);
    [Header("범위 표시")]
    [SerializeField] private string _areaIndicatorKey = "PotionAreaIndicator";
    [SerializeField] private float _areaIndicatorDuration = 0.6f;
    List<Collider2D> _hitBuffer = new List<Collider2D>(15);
    private ContactFilter2D _contactFilter;

    private void Awake()
    {
        _pooled = GetComponent<PooledObject>();


        _contactFilter = new ContactFilter2D();
        _contactFilter.useTriggers = true;
        
        OnAwake();
    }

    // 자식 클래스에서 Awake 추가 초기화가 필요하면 오버라이드
    protected virtual void OnAwake() { }

    public void Initialize(PlayerStat stat, LayerMask layerMask)
    {
        _playerStat = stat;
        // layerMask가 들어온 후 contactFilter 갱신
        _contactFilter.SetLayerMask(layerMask);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == _playerStat.gameObject) return;

        _hitBuffer.Clear();
        int count = Physics2D.OverlapBox(
            transform.position, _overlapSize,
            0f, _contactFilter, _hitBuffer
        );

        for (int i = 0; i < count; i++)
        {
            ApplyEffect(_hitBuffer[i]);
        }

        ShowAreaIndicator();
        _pooled.ReturnToPool();
    }

    // 자식 클래스에서 효과 구현
    protected abstract void ApplyEffect(Collider2D hit);

    protected virtual Color AreaColor => Color.white;

    private void ShowAreaIndicator()
    {
        ObjectPoolManager poolManager = ObjectPoolManager.Instance;
        if (poolManager == null)
            return;

        GameObject areaIndicator = poolManager.Get(_areaIndicatorKey, transform.position, Quaternion.identity);
        if (areaIndicator == null)
            return;

        Color color = AreaColor;
        WitchPotionAreaVisibleAttack visibleAttack = null;
        if (areaIndicator.TryGetComponent(out visibleAttack))
            visibleAttack.ShowPotionArea(transform.position, _overlapSize, color, _areaIndicatorDuration);

        if (areaIndicator.TryGetComponent<SelfReturn>(out var selfReturn))
            selfReturn.ReturnAfter(_areaIndicatorDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _overlapSize);
    }
}
