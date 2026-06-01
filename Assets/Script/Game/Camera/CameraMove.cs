using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float _positionSmoothTime = 0.15f;
    [SerializeField] private float _zoomSmoothTime = 0.15f;

    [Header("Zoom")]
    [SerializeField] private float _minSize = 6f;
    [SerializeField] private float _padding = 2f;

    private Transform _player1Transform;
    private Transform _player2Transform;

    private Camera _camera;
    private Vector3 _positionVelocity;
    private float _zoomVelocity;
    private float _zPos;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
            _camera = Camera.main;

        _zPos = transform.position.z;
    }

    public void SetTargets(Transform p1, Transform p2)
    {
        _player1Transform = p1;
        _player2Transform = p2;
    }

    void LateUpdate()
    {
        if (_player1Transform == null || _player2Transform == null) return;
        if (_camera == null) return;

        Vector3 p1Pos = _player1Transform.position;
        Vector3 p2Pos = _player2Transform.position;

        float xPos = (p1Pos.x + p2Pos.x) / 2f;
        float yPos = (p1Pos.y + p2Pos.y) / 2f;

        Vector3 targetPosition = new Vector3(xPos, yPos, _zPos);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _positionVelocity, _positionSmoothTime);

        float horizontalDistance = Mathf.Abs(p1Pos.x - p2Pos.x);
        float verticalDistance = Mathf.Abs(p1Pos.y - p2Pos.y);
        float verticalSize = verticalDistance * 0.5f + _padding;
        float horizontalSize = horizontalDistance * 0.5f / _camera.aspect + _padding;
        float targetSize = Mathf.Max(verticalSize, horizontalSize, _minSize);

        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, targetSize, ref _zoomVelocity, _zoomSmoothTime);
    }
}
