using UnityEngine;

public interface IInputProvider
{
    float GetMove();
    bool GetJump();
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private IInputProvider input;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public void Initialize(IInputProvider provider)
    {
        input = provider;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Initialize(GetComponent<IInputProvider>());
    }

    private void FixedUpdate()
    {
        if (input == null) return;

        float move = input.GetMove();

        rb.linearVelocity = new Vector2(
            move * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void Update()
    {
        if (input == null) return;

        if (input.GetJump())
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }
    }
}