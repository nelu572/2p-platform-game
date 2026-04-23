using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("점프")]
    [SerializeField] private float _jumpForce = 10f;

    [Header("지면 감지")]
    [SerializeField] private LayerMask _groundLayer;
    //캐싱을 위한 변수
    private Vector2 boxSize;

    public Action OnAttackHandler;
    public Action OnSkillHandler;

    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;

    // 점프 공격과 일반공격 애니메이션 구별하기 위해 public으로 변경
    public bool _isGrounded;

    public int TeamId { get; set; }
    //넉백을 위해 만든 프로퍼티
    public bool IsKnockedBack { get; set; }
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;
        _boxCollider2D = GetComponent<BoxCollider2D>();

        boxSize = _boxCollider2D.size;
    }
    
    void FixedUpdate()
    {
        Vector2 boxCenter = (Vector2)transform.position + _boxCollider2D.offset; // 지역변수
        Vector2 capsuleCenter = new Vector2(boxCenter.x, boxCenter.y - boxSize.y * 0.5f); // 지역변수

        _isGrounded = Physics2D.OverlapCapsule(
            capsuleCenter,
            new Vector2(boxSize.x * 0.9f, 0.1f),
            CapsuleDirection2D.Horizontal,
            0f,
            _groundLayer ) != null;
        
    }

    public void Move(Vector2 input)
    {
        if (IsKnockedBack) return;

        _rigidbody2D.linearVelocity = new Vector2(
            input.x * _moveSpeed,
            _rigidbody2D.linearVelocity.y  // ← 현재 y velocity 유지
        );

        if (input.x > 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (input.x < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    
    public void Jump()
    {
        if (_isGrounded)
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpForce);   
        }
    }
    
    public void Attack()
    {
        OnAttackHandler?.Invoke();
    }
    
    public void Skill()
    {
        OnSkillHandler?.Invoke();
    }

    public void TakeDamage(int attackDamage)
    {
        Debug.Log($"{TeamId} : 체력 감소");
    }
}