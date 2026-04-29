using DG.Tweening;
using UnityEngine;

/// <summary>
/// Fade 타입 패널의 열기/닫기 애니메이션만 담당한다.
/// </summary>
public class UIPanelFadePlayer : IUITransitionPlayer
{
    private readonly CanvasGroup _canvasGroup;

    public UIPanelFadePlayer(CanvasGroup canvasGroup)
    {
        _canvasGroup = canvasGroup;
    }

    public void Prepare(bool isOpening)
    {
        // Fade 시작 전에 alpha를 시작값으로 맞춰 둔다.
        _canvasGroup.alpha = isOpening ? 0f : 1f;
    }

    public bool CanPlay(PanelType panelType)
    {
        return panelType.HasFlag(PanelType.FadeInOut);
    }

    public Tween CreateTween(bool isOpening, PanelTransitionData transitionData)
    {
        float endAlpha = isOpening ? 1f : 0f;
        float duration = isOpening
            ? transitionData.FadeInDuration
            : transitionData.FadeOutDuration;

        return _canvasGroup.DOFade(endAlpha, duration);
    }
}
