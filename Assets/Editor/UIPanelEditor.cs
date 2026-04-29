using UnityEditor;

[CustomEditor(typeof(UIPanel))]
public class UIPanelEditor : Editor
{
    private SerializedProperty _panelDataProp;

    private SerializedProperty _dataTypeProp;
    private SerializedProperty _fadeDurationProp;
    private SerializedProperty _scaleDurationProp;
    private SerializedProperty _moveOffsetProp;

    private void OnEnable()
    {
        _panelDataProp = serializedObject.FindProperty("_panelTransitionData");

        _dataTypeProp = _panelDataProp.FindPropertyRelative("type");
        _fadeDurationProp = _panelDataProp.FindPropertyRelative("fadeDuration");
        _scaleDurationProp = _panelDataProp.FindPropertyRelative("scaleDuration");
        _moveOffsetProp = _panelDataProp.FindPropertyRelative("moveOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Transition Data", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_dataTypeProp);

        PanelType type = (PanelType)_dataTypeProp.intValue;

        if (type.HasFlag(PanelType.FadeInOut))
        {
            EditorGUILayout.PropertyField(_fadeDurationProp);
        }

        if (type.HasFlag(PanelType.ScaleInOut))
        {
            EditorGUILayout.PropertyField(_scaleDurationProp);
        }

        if (type.HasFlag(PanelType.Move))
        {
            EditorGUILayout.PropertyField(_moveOffsetProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}