using UnityEngine;

public class WarriorVisibleAttack : VisibleAttack
{
    [SerializeField] private Color _attackFillColor = new Color(1f, 0f, 0f, 0.28f);
    [SerializeField] private Color _attackOutlineColor = new Color(1f, 0f, 0f, 0.9f);
    [SerializeField] private float _attackDuration = 0.15f;

    public void ShowAttack(Vector2 center, Vector2 size)
    {
        ShowWorldShape(CreateBoxVertices(center, size), _attackFillColor, _attackOutlineColor);
        HideAfter(_attackDuration);
    }

    private Vector3[] CreateBoxVertices(Vector2 center, Vector2 size)
    {
        Vector2 halfSize = new Vector2(Mathf.Max(0f, size.x), Mathf.Max(0f, size.y)) * 0.5f;
        return new Vector3[]
        {
            new Vector3(center.x - halfSize.x, center.y - halfSize.y, 0f),
            new Vector3(center.x + halfSize.x, center.y - halfSize.y, 0f),
            new Vector3(center.x + halfSize.x, center.y + halfSize.y, 0f),
            new Vector3(center.x - halfSize.x, center.y + halfSize.y, 0f)
        };
    }
}
