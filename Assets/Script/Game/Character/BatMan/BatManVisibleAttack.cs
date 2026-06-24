using UnityEngine;

public class BatManVisibleAttack : VisibleAttack
{
    [SerializeField] private Color _minChargeColor = new Color(1f, 0.45f, 0f, 1f);
    [SerializeField] private Color _maxChargeColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float _minFillAlpha = 0.18f;
    [SerializeField] private float _maxFillAlpha = 0.42f;
    [SerializeField] private float _minOutlineAlpha = 0.65f;
    [SerializeField] private float _maxOutlineAlpha = 1f;
    [SerializeField] private float _releaseDuration = 0.18f;

    public void ShowCharging(Vector2 center, Vector2 size, float chargeTime, float maxChargeTime)
    {
        float chargeRatio = GetChargeRatio(chargeTime, maxChargeTime);
        Color color = GetChargeColor(chargeRatio);
        ShowWorldShape(
            CreateBoxVertices(center, size),
            WithAlpha(color, Mathf.Lerp(_minFillAlpha, _maxFillAlpha, chargeRatio)),
            WithAlpha(color, Mathf.Lerp(_minOutlineAlpha, _maxOutlineAlpha, chargeRatio))
        );
    }

    public void ShowRelease(Vector2 center, Vector2 size, float chargeTime, float maxChargeTime)
    {
        float chargeRatio = GetChargeRatio(chargeTime, maxChargeTime);
        Color color = GetChargeColor(chargeRatio);
        ShowWorldShape(CreateBoxVertices(center, size), WithAlpha(color, _maxFillAlpha), WithAlpha(color, _maxOutlineAlpha));
        HideAfter(_releaseDuration);
    }

    private float GetChargeRatio(float chargeTime, float maxChargeTime)
    {
        return maxChargeTime > 0f ? Mathf.Clamp01(chargeTime / maxChargeTime) : 1f;
    }

    private Color GetChargeColor(float chargeRatio)
    {
        return Color.Lerp(_minChargeColor, _maxChargeColor, chargeRatio);
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
