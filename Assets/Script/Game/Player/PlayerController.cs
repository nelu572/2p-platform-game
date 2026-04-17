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
    [SerializeField] private float _groundCheckRadius = 0.1f;
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
        _isGrounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );
    }
    public void Move(Vector2 input)
    {
        Vector2 move = new Vector2(input.x * _moveSpeed * Time.deltaTime, 0f);
        _rigidbody2D.MovePosition(_rigidbody2D.position + move);

        if (input.x > 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (input.x < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    public void Jump()
    {
        if (_isGrounded)
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpForce);

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