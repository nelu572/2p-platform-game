using System.Collections.Generic;
using UnityEngine;

public class WitchVisibleAttack : VisibleAttack
{
    [Header("투척 궤적")]
    [SerializeField] private Color _trajectoryColor = new Color(1f, 1f, 1f, 0.65f);
    [SerializeField] private int _trajectoryPointCount = 18;
    [SerializeField] private float _trajectoryTimeStep = 0.08f;
    [SerializeField] private float _trajectoryDotSize = 0.12f;
    [SerializeField] private float _trajectoryMaxDistance = 8f;
    private readonly List<Vector3> _cachedVertices = new List<Vector3>();

    public void ShowThrowTrajectory(Vector2 startPosition, Vector2 initialVelocity, float gravityScale)
    {
        int pointCount = Mathf.Max(2, _trajectoryPointCount);
        _cachedVertices.Clear();
        int requiredCapacity = pointCount * 4;
        if (_cachedVertices.Capacity < requiredCapacity)
            _cachedVertices.Capacity = requiredCapacity;

        Vector2 gravity = Physics2D.gravity * gravityScale;
        float maxDistance = Mathf.Max(0f, _trajectoryMaxDistance);

        for (int i = 0; i < pointCount; i++)
        {
            float time = i * Mathf.Max(0.01f, _trajectoryTimeStep);
            Vector2 position = startPosition + initialVelocity * time + 0.5f * gravity * time * time;
            Vector2 offset = position - startPosition;

            if (maxDistance > 0f && offset.magnitude > maxDistance)
            {
                position = startPosition + offset.normalized * maxDistance;
                AddDotVertices(_cachedVertices, position, _trajectoryDotSize);
                break;
            }

            AddDotVertices(_cachedVertices, position, _trajectoryDotSize);
        }

        ShowWorldFill(_cachedVertices, _trajectoryColor);
    }

    private void AddDotVertices(List<Vector3> vertices, Vector2 center, float size)
    {
        float halfSize = Mathf.Max(0.01f, size) * 0.5f;
        vertices.Add(new Vector3(center.x - halfSize, center.y - halfSize, 0f));
        vertices.Add(new Vector3(center.x + halfSize, center.y - halfSize, 0f));
        vertices.Add(new Vector3(center.x + halfSize, center.y + halfSize, 0f));
        vertices.Add(new Vector3(center.x - halfSize, center.y + halfSize, 0f));
    }
}
