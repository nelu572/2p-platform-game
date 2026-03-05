using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 8f;

    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask groundLayer;

    private bool isJumpHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) isJumpHeld = true;
        if (context.canceled) isJumpHeld = false;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

        if (isJumpHeld && getOnground())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
    }

    private bool getOnground()
    {
        LayerMask layerMask = objectLayer | groundLayer;

        Vector2 origin = transform.position;
        Vector2 size = GetComponent<Collider2D>().bounds.size;
        Vector2 direction = Vector2.down;
        float distance = 0.1f;

        // DrawBoxCast(origin, size, direction, distance); 레이박스 디버깅

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            origin,
            size,
            0f,
            direction,
            distance,
            layerMask
        );

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
                return true;
        }

        return false;
    }

    private void DrawBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
    {
        Vector2 half = size / 2f;
        Vector2 end = origin + direction * distance;

        Debug.DrawLine(origin + new Vector2(-half.x, half.y), origin + new Vector2(half.x, half.y), Color.yellow, 1f);
        Debug.DrawLine(origin + new Vector2(half.x, half.y), origin + new Vector2(half.x, -half.y), Color.yellow, 1f);
        Debug.DrawLine(origin + new Vector2(half.x, -half.y), origin + new Vector2(-half.x, -half.y), Color.yellow, 1f);
        Debug.DrawLine(origin + new Vector2(-half.x, -half.y), origin + new Vector2(-half.x, half.y), Color.yellow, 1f);

        Debug.DrawLine(end + new Vector2(-half.x, half.y), end + new Vector2(half.x, half.y), Color.red, 1f);
        Debug.DrawLine(end + new Vector2(half.x, half.y), end + new Vector2(half.x, -half.y), Color.red, 1f);
        Debug.DrawLine(end + new Vector2(half.x, -half.y), end + new Vector2(-half.x, -half.y), Color.red, 1f);
        Debug.DrawLine(end + new Vector2(-half.x, -half.y), end + new Vector2(-half.x, half.y), Color.red, 1f);

        Debug.DrawLine(origin, end, Color.green, 1f);
    }
}