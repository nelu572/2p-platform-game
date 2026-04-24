using UnityEngine;

// 검기 발사체
public class SlashWave : MonoBehaviour
{
    //공격력
    private int _damage;
    //같은 팀인지 구별하기 위한 Id
    private int _ownerTeamId;
    // 검기 속도
    private float _speed;
    // 검기의 방향(플레이어가 보고 있는 방향)
    private Vector2 _direction;
    // 검기의 최대 거리
    private float _maxDistance;
    //검기가 시작된 지점
    private Vector2 _startPosition;
    //검기에 맞을 레이어
    private LayerMask _layerMask;

    [SerializeField] private float _knockbackForce = 8f;

    public void Initialize(int damage, int ownerTeamId, Vector2 direction, float speed, float maxDistance, LayerMask layerMask)
    {
        _damage = damage;
        _ownerTeamId = ownerTeamId;
        _direction = direction.normalized;
        _speed = speed;
        _maxDistance = maxDistance;
        _layerMask = layerMask;
        _startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);

        // 시작 위치로부터 거리 초과 시 제거
        if (Vector2.Distance(transform.position, _startPosition) >= _maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 충돌한 오브젝트의 레이어가 _layerMask에 포함되는지 확인
        if ((_layerMask.value & (1 << col.gameObject.layer)) == 0) return;

        if (col.TryGetComponent<IDamageable>(out var damageable))
        {
            // 다른 팀만 적용
            if (damageable.TeamId == _ownerTeamId) return;

            damageable.TakeDamage(_damage);

                if (col.TryGetComponent<IKnockbackable>(out var pc))
                    pc.ApplyKnockback(_direction, _knockbackForce);
        }
    }
}