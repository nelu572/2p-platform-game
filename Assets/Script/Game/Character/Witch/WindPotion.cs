using UnityEngine;

public class WindPotion : Potion
{
    protected override Color AreaColor => new Color(0.2f, 1f, 0.95f, 1f);

    //점프 공격이기에 하늘에 있을 가능성을 염두하고 높게 잡음
    [SerializeField] private Vector2 _size = new Vector2(3f, 15f);
    [SerializeField] private float _jumpAttackUpForce = 10f;

    // OnAwake에서 _overlapSize를 _size로 설정, Potion의 Awake에서 호출됨
    protected override void OnAwake()
    {
        _overlapSize = _size;
    }
    protected override void ApplyEffect(Collider2D hit)
    {
        if(hit.TryGetComponent<PlayerController>(out var player))
        {
            if(player.TryGetComponent<Rigidbody2D>(out var rb))
            {
                //바람 포션의 힘과 방향 설정
                Vector2 forceDirection = Vector2.up; // 위쪽 방향으로 힘을 가함
                Vector2 force = forceDirection * _jumpAttackUpForce;
                player.ResetFallSpeed(); 
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

}
