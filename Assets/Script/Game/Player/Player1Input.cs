using UnityEngine;

public class Player1Input : MonoBehaviour, IInputProvider
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask objectLayer;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public float GetMove()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.D)) move = 1f;
        return move;
    }

    public bool GetJump()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public bool GetOnGround()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            0.05f,
            groundLayer | objectLayer
        );

        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            if (hit.collider == col)   // 자기 자신이면 무시
                continue;

            return true;               // 자기 자신이 아닌 무언가를 맞으면 true
        }

        return false;
    }
}