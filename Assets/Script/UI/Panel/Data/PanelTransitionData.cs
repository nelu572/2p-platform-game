using UnityEngine;

[System.Serializable]
public class PanelTransitionData
{
    // 패널 애니메이션 타입
    public PanelType Type;

    // FadeInOut 시간 값
    public float FadeInDuration = 0.3f;
    public float FadeOutDuration = 0.3f;

    // ScaleInOut 시간 값
    public float ScaleInDuration = 0.3f;
    public float ScaleOutDuration = 0.3f;

    // Move 설정 값
    // todo : Move안에 enum 타입을 하나 만들어서 좌표, 오프셋 등 기준을 여러개 만들기
    public Vector3 MoveOffset = Vector3.zero;
    public float MoveTime = 0.3f;
}
