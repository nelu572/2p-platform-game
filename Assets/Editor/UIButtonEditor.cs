using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(UIButton), true)]
[CanEditMultipleObjects]
public class UIButtonEditor : ButtonEditor
{
    private SerializedProperty _clickSound;
    private SerializedProperty _hoverSound;

    private SerializedProperty _buttonType;
    private SerializedProperty _enablePanel;
    private SerializedProperty _disablePanel;
    private SerializedProperty _nextSceneName;
    private SerializedProperty _upButton;
    private SerializedProperty _downButton;
    private SerializedProperty _leftButton;
    private SerializedProperty _rightButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        _clickSound = serializedObject.FindProperty("_clickSound");
        _hoverSound = serializedObject.FindProperty("_hoverSound");

        _buttonType = serializedObject.FindProperty("_buttonType");
        _enablePanel = serializedObject.FindProperty("_enablePanel");
        _disablePanel = serializedObject.FindProperty("_disablePanel");
        _nextSceneName = serializedObject.FindProperty("_nextSceneName");
        _upButton = serializedObject.FindProperty("_upButton");
        _downButton = serializedObject.FindProperty("_downButton");
        _leftButton = serializedObject.FindProperty("_leftButton");
        _rightButton = serializedObject.FindProperty("_rightButton");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ===== 사운드 =====
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sound", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_clickSound);
        EditorGUILayout.PropertyField(_hoverSound);

        // ===== 버튼 로직 =====
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Button Logic", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_buttonType);

        ButtonType type = (ButtonType)_buttonType.enumValueIndex;

        switch (type)
        {
            case ButtonType.ChangePanel:
                EditorGUILayout.PropertyField(_disablePanel, new GUIContent("Object To Disable"));
                EditorGUILayout.PropertyField(_enablePanel, new GUIContent("Object To Enable"));
                break;

            case ButtonType.OpenPopup:
                EditorGUILayout.PropertyField(_enablePanel, new GUIContent("Popup To Open"));
                break;

            case ButtonType.ClosePopup:
                EditorGUILayout.PropertyField(_disablePanel, new GUIContent("Popup To Close"));
                break;

            case ButtonType.GoScene:
                EditorGUILayout.PropertyField(_nextSceneName, new GUIContent("Scene Name"));
                break;

            case ButtonType.Quit:
                EditorGUILayout.HelpBox("게임 종료 버튼", MessageType.None);
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Directional Navigation", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_upButton);
        EditorGUILayout.PropertyField(_downButton);
        EditorGUILayout.PropertyField(_leftButton);
        EditorGUILayout.PropertyField(_rightButton);

        serializedObject.ApplyModifiedProperties();

        // ===== 기본 Button =====
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Base Button", EditorStyles.boldLabel);
        base.OnInspectorGUI();
    }
}
