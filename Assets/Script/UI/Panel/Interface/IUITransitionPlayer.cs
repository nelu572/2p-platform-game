using DG.Tweening;

/// <summary>
/// 패널 타입 플레이어 클래스의 인터페이스
/// </summary>
public interface IUITransitionPlayer
{
    bool CanPlay(PanelType panelType);
    void Prepare(bool isOpening, PanelTransitionData transitionData);
    Tween CreateTween(bool isOpening, PanelTransitionData transitionData);
}
