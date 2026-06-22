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
        float angle = Mathf.Atan2(safeDirection.y, safeDirection.x) * Mathf.Rad2Deg;
        float chargeRatio = maxChargeTime > 0f ? Mathf.Clamp01(chargeTime / maxChargeTime) : 1f;
        float baseWidth = Mathf.Lerp(maxBaseWidth, minBaseWidth, chargeRatio);
        float halfBaseWidth = Mathf.Max(0f, baseWidth) * 0.5f;
        float displayLength = Mathf.Max(0f, hitLength * lengthScale + lengthOffset);
        Color color = GetChargeColor(isSkill, chargeRatio);

        ShowShape(
            apex,
            angle,
            CreateTriangleVertices(displayLength, halfBaseWidth),
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

    private Vector3[] CreateTriangleVertices(float displayLength, float halfBaseWidth)
    {
        return new Vector3[]
        {
            Vector3.zero,
            new Vector3(displayLength, -halfBaseWidth, 0f),
            new Vector3(displayLength, halfBaseWidth, 0f)
        };
    }
}
