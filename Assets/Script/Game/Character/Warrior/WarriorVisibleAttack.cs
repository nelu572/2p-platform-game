using UnityEngine;

public class WarriorVisibleAttack : VisibleAttack
{
    [SerializeField] private Color _attackFillColor = new Color(1f, 0f, 0f, 0.28f);
    [SerializeField] private Color _attackOutlineColor = new Color(1f, 0f, 0f, 0.9f);
    [SerializeField] private float _attackDuration = 0.15f;

    public void ShowAttack(Vector2 center, Vector2 size)
    {
        ShowWorldShape(GetBoxVertices(center, size), _attackFillColor, _attackOutlineColor);
        HideAfter(_attackDuration);
    }
}
