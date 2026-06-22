using UnityEngine;

public class WitchVisibleAttack : VisibleAttack
{
    [Header("투척 궤적")]
    [SerializeField] private Color _trajectoryColor = new Color(1f, 1f, 1f, 0.65f);
    [SerializeField] private int _trajectoryPointCount = 18;
    [SerializeField] private float _trajectoryTimeStep = 0.08f;
    [SerializeField] private float _trajectoryDotSize = 0.12f;

    public void ShowPotionArea(Vector2 center, Vector2 size, Color areaColor, float duration)
    {
        ShowShape(center, 0f, CreateBoxVertices(size), WithAlpha(areaColor, 0.26f), WithAlpha(areaColor, 0.9f));
        HideAfter(duration);
    }

    public void ShowThrowTrajectory(Vector2 startPosition, Vector2 initialVelocity, float gravityScale)
    {
        int pointCount = Mathf.Max(2, _trajectoryPointCount);
        Vector3[] vertices = new Vector3[pointCount * 4];
        Vector2 gravity = Physics2D.gravity * gravityScale;

        for (int i = 0; i < pointCount; i++)
        {
            float time = i * Mathf.Max(0.01f, _trajectoryTimeStep);
            Vector2 position = startPosition + initialVelocity * time + 0.5f * gravity * time * time;
            AddDotVertices(vertices, i * 4, position, _trajectoryDotSize);
        }

        ShowFill(Vector2.zero, 0f, vertices, _trajectoryColor);
    }

    private Vector3[] CreateBoxVertices(Vector2 size)
    {
        Vector2 halfSize = new Vector2(Mathf.Max(0f, size.x), Mathf.Max(0f, size.y)) * 0.5f;
        return new Vector3[]
        {
            new Vector3(-halfSize.x, -halfSize.y, 0f),
            new Vector3(halfSize.x, -halfSize.y, 0f),
            new Vector3(halfSize.x, halfSize.y, 0f),
            new Vector3(-halfSize.x, halfSize.y, 0f)
        };
    }

    private void AddDotVertices(Vector3[] vertices, int startIndex, Vector2 center, float size)
    {
        float halfSize = Mathf.Max(0.01f, size) * 0.5f;
        vertices[startIndex] = new Vector3(center.x - halfSize, center.y - halfSize, 0f);
        vertices[startIndex + 1] = new Vector3(center.x + halfSize, center.y - halfSize, 0f);
        vertices[startIndex + 2] = new Vector3(center.x + halfSize, center.y + halfSize, 0f);
        vertices[startIndex + 3] = new Vector3(center.x - halfSize, center.y + halfSize, 0f);
    }
}
