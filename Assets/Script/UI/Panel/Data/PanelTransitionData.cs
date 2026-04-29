using UnityEngine;

[System.Serializable]
public class PanelTransitionData
{
    // 패널 애니메이션 타입
    public PanelType Type;

    // Fade 전환 전용 설정
    public FadeTransitionData Fade = new FadeTransitionData();

    // Scale 전환 전용 설정
    public ScaleTransitionData Scale = new ScaleTransitionData();

    // Move 전환 전용 설정
    public MoveTransitionData Move = new MoveTransitionData();
}
