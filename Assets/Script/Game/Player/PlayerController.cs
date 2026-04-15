using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("점프")]
    [SerializeField] private float _jumpForce = 10f;

    [Header("지면 감지")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rigidbody2D;
    private bool _isGrounded;
    private bool _jumpRequested;
    private float _moveInput;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;
    }
    void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );
    }
    void FixedUpdate()
    {
        if (_jumpRequested && _isGrounded)
        {
            _rigidbody2D.linearVelocity = new Vector2(0f, _jumpForce);
            _jumpRequested = false;
        }
    }

    public void Move(Vector2 input)
    {
        _moveInput = input.x;

        Vector2 move = new Vector2(input.x * _moveSpeed * Time.fixedDeltaTime, 0f);
        _rigidbody2D.MovePosition(_rigidbody2D.position + move);

        if (input.x > 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (input.x < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    public void Jump()
    {
        if (_isGrounded)
            _jumpRequested = true;
    }
    public void Attack()
    {
        Debug.Log("공격 실행");
    }
    public void Skill()
    {
        Debug.Log("스킬 실행");
    }
    //// 에디터에서 지면 감지 범위 시각화
    //void OnDrawGizmosSelected()
    //{
    //    if (groundCheck == null) return;
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    //}
}