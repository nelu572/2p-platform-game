using UnityEngine;

[System.Serializable]
public class MoveTransitionData
{
    // todo : Move안에 enum 타입을 하나 만들어서 좌표, 오프셋 등 기준을 여러개 만들기
    public Vector3 Offset = Vector3.zero;
    public float Duration = 0.3f;
}
