using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform player1;
    private Transform player2;

    float z_pos = -10f;

    public void SetTargets(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        float x_pos = (player1.position.x + player2.position.x) / 2f;
        float y_pos = (player1.position.y + player2.position.y) / 2f;

        transform.position = new Vector3(x_pos, y_pos, z_pos);

        float distance = Vector2.Distance(player1.position, player2.position);
        Camera.main.orthographicSize = Mathf.Max(distance * 0.5f, 6f);
    }
}