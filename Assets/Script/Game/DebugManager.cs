#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class DebugManager : MonoBehaviour
{
    private const KeyCode CursorToggleKey = KeyCode.M;
    private const KeyCode HelpToggleKey = KeyCode.F1;
    private const float HelpBoxWidth = 260f;
    private const float HelpBoxHeight = 120f;

    private static DebugManager _instance;

    private bool _isCursorVisible;
    private bool _isHelpVisible;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateDebugManager()
    {
        if (_instance != null)
            return;

        GameObject obj = new GameObject("[DebugManager]");
        _instance = obj.AddComponent<DebugManager>();

        DontDestroyOnLoad(obj);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        SetCursorVisible(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(CursorToggleKey))
            SetCursorVisible(!_isCursorVisible);

        if (Input.GetKeyDown(HelpToggleKey))
            _isHelpVisible = !_isHelpVisible;
    }

    private void OnGUI()
    {
        if (!_isHelpVisible)
            return;

        Rect helpRect = new Rect(12f, 12f, HelpBoxWidth, HelpBoxHeight);
        string sceneName = SceneManager.GetActiveScene().name;
        string cursorState = _isCursorVisible ? "켜짐" : "꺼짐";

        GUI.Box(helpRect, "디버그 도움말");
        GUILayout.BeginArea(new Rect(24f, 38f, HelpBoxWidth - 24f, HelpBoxHeight - 24f));
        GUILayout.Label($"현재 씬: {sceneName}");
        GUILayout.Label($"마우스 커서: {cursorState}");
        GUILayout.Label("M: 마우스 커서 켜기/끄기");
        GUILayout.Label("F1: 디버그 도움말 켜기/끄기");
        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void SetCursorVisible(bool visible)
    {
        _isCursorVisible = visible;
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
#endif
