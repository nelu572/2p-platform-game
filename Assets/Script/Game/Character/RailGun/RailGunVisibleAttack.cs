using UnityEngine;

public class RailGunVisibleAttack : VisibleAttack
{
    [SerializeField] private Color _attackMinColor = new Color(1f, 0.9f, 0f, 1f);
    [SerializeField] private Color _attackMaxColor = new Color(1f, 0.25f, 0f, 1f);
    [SerializeField] private Color _skillMinColor = new Color(0f, 1f, 1f, 1f);
    [SerializeField] private Color _skillMaxColor = new Color(0f, 0.35f, 1f, 1f);
    [SerializeField] private float _minFillAlpha = 0.16f;
    [SerializeField] private float _maxFillAlpha = 0.42f;
    [SerializeField] private float _minOutlineAlpha = 0.6f;
    [SerializeField] private float _maxOutlineAlpha = 1f;

    public void ShowCharge(
        Vector2 apex,
        Vector2 facingDirection,
        float hitLength,
        float lengthScale,
        float lengthOffset,
        float maxBaseWidth,
        float minBaseWidth,
        float chargeTime,
        float maxChargeTime,
        bool isSkill)
    {
        Vector2 safeDirection = facingDirection.sqrMagnitude > 0f ? facingDirection.normalized : Vector2.right;
        float chargeRatio = maxChargeTime > 0f ? Mathf.Clamp01(chargeTime / maxChargeTime) : 1f;
        float baseWidth = Mathf.Lerp(maxBaseWidth, minBaseWidth, chargeRatio);
        float halfBaseWidth = Mathf.Max(0f, baseWidth) * 0.5f;
        float displayLength = Mathf.Max(0f, hitLength * lengthScale + lengthOffset);
        Color color = GetChargeColor(isSkill, chargeRatio);

        ShowWorldShape(
            CreateTriangleVertices(apex, safeDirection, displayLength, halfBaseWidth),
            WithAlpha(color, Mathf.Lerp(_minFillAlpha, _maxFillAlpha, chargeRatio)),
            WithAlpha(color, Mathf.Lerp(_minOutlineAlpha, _maxOutlineAlpha, chargeRatio))
        );
    }

    private Color GetChargeColor(bool isSkill, float chargeRatio)
    {
        return isSkill
            ? Color.Lerp(_skillMinColor, _skillMaxColor, chargeRatio)
            : Color.Lerp(_attackMinColor, _attackMaxColor, chargeRatio);
    }

    private Vector3[] CreateTriangleVertices(Vector2 apex, Vector2 direction, float displayLength, float halfBaseWidth)
    {
        Vector2 baseCenter = apex + direction * displayLength;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        return new Vector3[]
        {
            new Vector3(apex.x, apex.y, 0f),
            new Vector3(baseCenter.x - perpendicular.x * halfBaseWidth, baseCenter.y - perpendicular.y * halfBaseWidth, 0f),
            new Vector3(baseCenter.x + perpendicular.x * halfBaseWidth, baseCenter.y + perpendicular.y * halfBaseWidth, 0f)
        };
    }
}
