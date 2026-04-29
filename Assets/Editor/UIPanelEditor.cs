using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIPanel))]
public class UIPanelEditor : Editor
{
    private bool _isReady;

    private SerializedProperty _canvasGroupProp;
    private SerializedProperty _baseEnabledProp;

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
        _isReady = false;

        if (targets == null || targets.Length == 0 || targets[0] == null)
        {
            return;
        }

        _canvasGroupProp = serializedObject.FindProperty("_canvasGroup");
        _baseEnabledProp = serializedObject.FindProperty("_baseEnabled");

        _panelDataProp = serializedObject.FindProperty("_panelTransitionData");

        if (_panelDataProp == null)
        {
            return;
        }

        _dataTypeProp = _panelDataProp.FindPropertyRelative("Type");

        SerializedProperty fadeProp = _panelDataProp.FindPropertyRelative("Fade");
        SerializedProperty scaleProp = _panelDataProp.FindPropertyRelative("Scale");
        SerializedProperty moveProp = _panelDataProp.FindPropertyRelative("Move");

        _fadeInDurationProp = fadeProp.FindPropertyRelative("InDuration");
        _fadeOutDurationProp = fadeProp.FindPropertyRelative("OutDuration");

        _scaleInDurationProp = scaleProp.FindPropertyRelative("InDuration");
        _scaleOutDurationProp = scaleProp.FindPropertyRelative("OutDuration");

        _moveOffsetProp = moveProp.FindPropertyRelative("Offset");
        _moveTimeProp = moveProp.FindPropertyRelative("Duration");

        _isReady = true;
    }

    public override void OnInspectorGUI()
    {
        if (_isReady == false || target == null)
        {
            return;
        }

        serializedObject.Update();

        EditorGUILayout.LabelField("Base", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_canvasGroupProp);
        EditorGUILayout.PropertyField(_baseEnabledProp);
        EditorGUILayout.Space(6);

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
            DrawSection(
                "Fade",
                _fadeInDurationProp, "In Duration",
                _fadeOutDurationProp, "Out Duration");
        }

        if (type.HasFlag(PanelType.ScaleInOut))
        {
            DrawSection(
                "Scale",
                _scaleInDurationProp, "In Duration",
                _scaleOutDurationProp, "Out Duration");
        }

        if (type.HasFlag(PanelType.Move))
        {
            DrawSection(
                "Move",
                _moveOffsetProp, "Offset",
                _moveTimeProp, "Duration");
        }

        serializedObject.ApplyModifiedProperties();
    }

    // 인스펙터 레이아웃은 공통으로 두고, 섹션별 표시 값만 넘겨서 반복을 줄인다.
    private void DrawSection(
        string title,
        SerializedProperty firstProp,
        string firstLabel,
        SerializedProperty secondProp,
        string secondLabel)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(firstProp, new GUIContent(firstLabel));
        EditorGUILayout.PropertyField(secondProp, new GUIContent(secondLabel));
        EditorGUI.indentLevel--;

        GUILayout.EndVertical();
    }
}
