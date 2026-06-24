using UnityEngine;

public class WitchPotionAreaVisibleAttack : VisibleAttack
{
    public void ShowPotionArea(Vector2 center, Vector2 size, Color areaColor, float duration)
    {
        ShowWorldShape(CreateBoxVertices(center, size), WithAlpha(areaColor, 0.26f), WithAlpha(areaColor, 0.9f));
        HideAfter(duration);
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
