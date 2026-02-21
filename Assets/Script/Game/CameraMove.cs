using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    float z_pos = -10f;
    void LateUpdate()
    {
        float x_pos = (player1.position.x + player2.position.x) / 2;
        float y_pos = (player1.position.y + player2.position.y) / 2;
        Vector3 pos = new Vector3(x_pos, y_pos, z_pos);
        transform.position = pos;

        float distance = Vector2.Distance(player1.position, player2.position);
        Camera.main.orthographicSize = Mathf.Max(distance * 0.5f, 6);
    }
}
