using UnityEngine;

public abstract class Potion : MonoBehaviour
{
    [SerializeField] private PooledObject _pooled;
    [SerializeField] protected PlayerStat PlayerStat;
    [SerializeField] protected LayerMask AttackLayerMask;

    [Header("공격 감지")]
    [SerializeField] private Vector2 _overlapSize = new Vector2(1f, 1f);

    private void Awake()
    {
        _pooled = GetComponent<PooledObject>();
        OnAwake();
    }

    // 자식 클래스에서 Awake 추가 초기화가 필요하면 오버라이드
    protected virtual void OnAwake() { }

    public void Initialize(PlayerStat stat, LayerMask layerMask)
    {
        PlayerStat = stat;
        AttackLayerMask = layerMask;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            _overlapSize,
            0f,
            AttackLayerMask
        );

        foreach (var hit in hits)
        {
            ApplyEffect(hit);
        }

        _pooled.ReturnToPool();
    }

    // 자식 클래스에서 효과 구현
    protected abstract void ApplyEffect(Collider2D hit);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _overlapSize);
    }
}