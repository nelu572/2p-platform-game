using DG.Tweening;
using UnityEngine;

/// <summary>
/// Move 타입 패널의 열기/닫기 애니메이션만 담당한다.
/// </summary>
public class UIPanelMovePlayer : IUITransitionPlayer
{
    private readonly RectTransform _rectTransform;
    private readonly Vector2 _openedPosition;

    public UIPanelMovePlayer(RectTransform rectTransform)
    {
        _rectTransform = rectTransform;
        // 현재 위치를 패널이 완전히 열린 기준 위치로 사용한다.
        _openedPosition = rectTransform.anchoredPosition;
    }

    public bool CanPlay(PanelType panelType)
    {
        return panelType.HasFlag(PanelType.Move);
    }

    public void Prepare(bool isOpening, PanelTransitionData transitionData)
    {
        // 현재 위치를 기준으로 MoveOffset만큼 떨어진 곳을 닫힌 위치로 사용한다.
        Vector2 closedPosition = _openedPosition + (Vector2)transitionData.Move.Offset;
        _rectTransform.anchoredPosition = isOpening ? closedPosition : _openedPosition;
    }

    public Tween CreateTween(bool isOpening, PanelTransitionData transitionData)
    {
        Vector2 closedPosition = _openedPosition + (Vector2)transitionData.Move.Offset;
        Vector2 endPosition = isOpening ? _openedPosition : closedPosition;

        return _rectTransform.DOAnchorPos(endPosition, transitionData.Move.Duration);
    }
}
