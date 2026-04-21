using UnityEngine;
using System;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour, IAttackController
{
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("점프")]
    [SerializeField] private float _jumpForce = 10f;

    [Header("지면 감지")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.6f;
    [SerializeField] private LayerMask _groundLayer;
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }
    
    protected Action OnAttackHandler;
    protected Action OnSkillHandler;

    private Rigidbody2D _rigidbody2D;
    private bool _isGrounded;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;
    }
    void Update()
    {
        if(AttackCooltime > 0)
            AttackCooltime -= Time.deltaTime;
        
        if(SkillCooltime > 0)
            SkillCooltime -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer) != null)
        {
            _isGrounded = true;
        }
    }
    public void Move(Vector2 input)
    {
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
            _isGrounded = false;
        }
    }
    public void Attack()
    {
        if (AttackCooltime > 0f)
            return;
        OnAttackHandler();
    }
    public void Skill()
    {
        if (SkillCooltime > 0f)
            return;
        OnSkillHandler();
    }
    //// 에디터에서 지면 감지 범위 시각화
    //void OnDrawGizmosSelected()
    //{
    //    if (groundCheck == null) return;
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    //}
}