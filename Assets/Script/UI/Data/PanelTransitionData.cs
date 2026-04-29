using UnityEngine;

[System.Serializable]
public class PanelTransitionData
{
    public PanelType type;

    public float fadeDuration = 0.3f;
    public float scaleDuration = 0.3f;
    public Vector3 moveOffset = Vector3.zero;
}