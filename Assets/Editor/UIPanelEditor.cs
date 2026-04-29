using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPanel))]
public class UIPanelEditor : Editor
{
    private SerializedProperty _panelDataProp;

    private SerializedProperty _dataTypeProp;

    /// <summary> FadeInOut필드 </summary>
    private SerializedProperty _fadeInDurationProp;
    private SerializedProperty _fadeOutDurationProp;

    /// <summary> ScaleInOut필드 </summary>
    private SerializedProperty _scaleInDurationProp;
    private SerializedProperty _scaleOutDurationProp;

    /// <summary> Move필드 </summary>
    private SerializedProperty _moveOffsetProp;
    private SerializedProperty _moveTimeProp;

    private void OnEnable()
    {
        _panelDataProp = serializedObject.FindProperty("_panelTransitionData");

        _dataTypeProp = _panelDataProp.FindPropertyRelative("Type");

        _fadeInDurationProp = _panelDataProp.FindPropertyRelative("FadeInDuration");
        _fadeOutDurationProp = _panelDataProp.FindPropertyRelative("FadeOutDuration");

        _scaleInDurationProp = _panelDataProp.FindPropertyRelative("ScaleInDuration");
        _scaleOutDurationProp = _panelDataProp.FindPropertyRelative("ScaleOutDuration");

        _moveOffsetProp = _panelDataProp.FindPropertyRelative("MoveOffset");
        _moveTimeProp = _panelDataProp.FindPropertyRelative("MoveTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

        PanelType type = (PanelType)_dataTypeProp.intValue;
        EditorGUI.BeginChangeCheck();
        type = (PanelType)EditorGUILayout.EnumFlagsField("Type", type);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Change UIPanel Type");
            _dataTypeProp.intValue = (int)type;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space(3);

        if (type.HasFlag(PanelType.FadeInOut))
        {
            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Fade", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_fadeInDurationProp, new GUIContent("In Duration"));
            EditorGUILayout.PropertyField(_fadeOutDurationProp, new GUIContent("Out Duration"));
            EditorGUI.indentLevel--;

            GUILayout.EndVertical();
        }

        if (type.HasFlag(PanelType.ScaleInOut))
        {
            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_scaleInDurationProp, new GUIContent("In Duration"));
            EditorGUILayout.PropertyField(_scaleOutDurationProp, new GUIContent("Out Duration"));
            EditorGUI.indentLevel--;

            GUILayout.EndVertical();
        }

        if (type.HasFlag(PanelType.Move))
        {
            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Move", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_moveOffsetProp, new GUIContent("Start Pos"));
            EditorGUILayout.PropertyField(_moveTimeProp, new GUIContent("Duration"));
            EditorGUI.indentLevel--;

            GUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
