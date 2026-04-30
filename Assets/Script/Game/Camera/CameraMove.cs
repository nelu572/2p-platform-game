using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Transform _player1Transform;
    private Transform _player2Transform;

    private float _zPos = -10f;

    public void SetTargets(Transform p1, Transform p2)
    {
        _player1Transform = p1;
        _player2Transform = p2;
    }

    void LateUpdate()
    {
        if (_player1Transform == null || _player2Transform == null) return;

        Vector3 p1Pos = _player1Transform.position;
        Vector3 p2Pos = _player2Transform.position;

        float x_pos = (p1Pos.x + p2Pos.x) / 2f;
        float y_pos = (p1Pos.y + p2Pos.y) / 2f;

        transform.position = new Vector3(x_pos, y_pos, _zPos);

        float distance = Vector2.Distance(p1Pos, p2Pos);
        Camera.main.orthographicSize = Mathf.Max(distance * 0.5f, 6f);
    }
}