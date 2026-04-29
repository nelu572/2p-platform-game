using DG.Tweening;
using UnityEngine;

/// <summary>
/// Scale 타입 패널의 열기/닫기 애니메이션만 담당한다.
/// </summary>
public class UIPanelScalePlayer : IUITransitionPlayer
{
    private readonly RectTransform _rectTransform;

    public UIPanelScalePlayer(RectTransform rectTransform)
    {
        _rectTransform = rectTransform;
    }

    public bool CanPlay(PanelType panelType)
    {
        return panelType.HasFlag(PanelType.ScaleInOut);
    }

    public void Prepare(bool isOpening)
    {
        // Scale 시작 전에 localScale을 시작값으로 맞춰 둔다.
        _rectTransform.localScale = isOpening ? Vector3.zero : Vector3.one;
    }

    public Tween CreateTween(bool isOpening, PanelTransitionData transitionData)
    {
        Vector3 endScale = isOpening ? Vector3.one : Vector3.zero;
        float duration = isOpening
            ? transitionData.ScaleInDuration
            : transitionData.ScaleOutDuration;

        return _rectTransform.DOScale(endScale, duration);
    }
}
