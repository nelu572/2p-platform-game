using UnityEngine;

public class WitchPotionAreaVisibleAttack : VisibleAttack
{
    public void ShowPotionArea(Vector2 center, Vector2 size, Color areaColor, float duration)
    {
        ShowWorldShape(GetBoxVertices(center, size), WithAlpha(areaColor, 0.26f), WithAlpha(areaColor, 0.9f));
        HideAfter(duration);
    }
}
