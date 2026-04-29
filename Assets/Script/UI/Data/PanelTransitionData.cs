using UnityEngine;

[System.Serializable]
public class PanelTransitionData
{
    public PanelType Type;

    public float FadeInDuration = 0.3f;
    public float FadeOutDuration = 0.3f;

    public float ScaleInDuration = 0.3f;
    public float ScaleOutDuration = 0.3f;

    public Vector3 MoveOffset = Vector3.zero;
    public float MoveTime = 0.3f;
}