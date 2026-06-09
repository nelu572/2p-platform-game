#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class DebugManager : MonoBehaviour
{
    [Header("커서 활성화 키")]
    [SerializeField]private const KeyCode CursorToggleKey = KeyCode.M;
    [SerializeField] private bool _isCursorVisible;

    [Header("디버그 도움말 창")]
    [SerializeField] private const float HelpBoxWidth = 430f;
    [SerializeField] private const float HelpBoxHeight = 230f;
    [SerializeField] private const KeyCode HelpToggleKey = KeyCode.F1;
    [SerializeField] private bool _isHelpVisible;

    [Header("상태 확인 창")]
    [SerializeField] private const float StatusBoxWidth = 430f;
    [SerializeField] private const float StatusBoxHeight = 165f;
    [SerializeField] private const KeyCode StatusToggleKey = KeyCode.F2;
    [SerializeField]private bool _isStatusVisible;
    
    [Header("로그 창")]
    [SerializeField]private const float LogBoxWidth = 860f;
    [SerializeField]private const float LogBoxHeight = 320f;
    [SerializeField]private const KeyCode LogToggleKey = KeyCode.F3;
    [SerializeField]private bool _isLogVisible;
    [SerializeField]private const int MaxLogCount = 30;
    [SerializeField]private const int VisibleLogCount = 8;
    private const int MaxLogMessageLength = 120;
    [SerializeField] private readonly List<LogEntry> _logs = new List<LogEntry>(MaxLogCount);

    private const float BoxPaddingX = 18f;
    private const float BoxTitleHeight = 40f;
    private const int LabelFontSize = 18;
    private const int BoxFontSize = 20;
    private const float FpsSmoothing = 0.1f;

    private float _smoothedDeltaTime;
    private GUIStyle _boxStyle;
    private GUIStyle _labelStyle;
    
    private static DebugManager _instance;

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

    private void OnEnable()
    {
        //로그가 찍힐떄마다 실행되는 이벤트 헨들러에 등록
        Application.logMessageReceived += HandleLogMessageReceived;
    }

    private void OnDisable()
    {
        //이 이벤트 헨들러는 static이라 이 메니저가 삭제되기 전에 미리 제거
        Application.logMessageReceived -= HandleLogMessageReceived;
    }

    private void Update()
    {
        UpdateFrameTime();

        if (Input.GetKeyDown(CursorToggleKey))
            SetCursorVisible(!_isCursorVisible);

        if (Input.GetKeyDown(HelpToggleKey))
            _isHelpVisible = !_isHelpVisible;

        if (Input.GetKeyDown(StatusToggleKey))
            _isStatusVisible = !_isStatusVisible;

        if (Input.GetKeyDown(LogToggleKey))
            _isLogVisible = !_isLogVisible;
    }
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void OnGUI()
    {
        EnsureStyles();

        if (_isHelpVisible)
            DrawHelpOverlay();

        if (_isStatusVisible)
            DrawStatusOverlay();

        if (_isLogVisible)
            DrawLogOverlay();
    }

    //디버그 도움말 구성
    private void DrawHelpOverlay()
    {
        Rect helpRect = new Rect(12f, 12f, HelpBoxWidth, HelpBoxHeight);

        string sceneName = SceneManager.GetActiveScene().name;
        string cursorState = _isCursorVisible ? "켜짐" : "꺼짐";

        GUI.Box(helpRect, "디버그 도움말", _boxStyle);
        GUILayout.BeginArea(GetContentRect(helpRect));
        GUILayout.Label($"현재 씬: {sceneName}", _labelStyle);
        GUILayout.Label($"마우스 커서: {cursorState}", _labelStyle);
        GUILayout.Label("M: 마우스 커서 켜기/끄기", _labelStyle);
        GUILayout.Label("F1: 디버그 도움말 켜기/끄기", _labelStyle);
        GUILayout.Label("F2: 상태 오버레이 켜기/끄기", _labelStyle);
        GUILayout.Label("F3: 로그 오버레이 켜기/끄기", _labelStyle);
        GUILayout.EndArea();
    }

    //상태창 구성
    private void DrawStatusOverlay()
    {
        Rect statusRect = new Rect(12f, 255f, StatusBoxWidth, StatusBoxHeight);
        string sceneName = SceneManager.GetActiveScene().name;
        float frameTimeMs = _smoothedDeltaTime * 1000f;
        float fps = _smoothedDeltaTime > 0f ? 1f / _smoothedDeltaTime : 0f;

        GUI.Box(statusRect, "상태 오버레이", _boxStyle);
        GUILayout.BeginArea(GetContentRect(statusRect));
        GUILayout.Label($"현재 씬: {sceneName}", _labelStyle);
        GUILayout.Label($"FPS: {fps:0.0} ({frameTimeMs:0.00} ms)", _labelStyle);
        GUILayout.Label($"해상도: {Screen.width} x {Screen.height}", _labelStyle);
        GUILayout.EndArea();
    }

    //디버그 창 구성
    private void DrawLogOverlay()
    {
        Rect logRect = new Rect(12f, 440f, LogBoxWidth, LogBoxHeight);

        GUI.Box(logRect, $"최근 로그 ({_logs.Count}/{MaxLogCount})", _boxStyle);
        GUILayout.BeginArea(GetContentRect(logRect));

        int startIndex = Mathf.Max(0, _logs.Count - VisibleLogCount);
        for (int i = startIndex; i < _logs.Count; i++)
        {
            LogEntry entry = _logs[i];
            Color previousColor = GUI.contentColor;
            GUI.contentColor = GetLogColor(entry.Type);
            GUILayout.Label($"[{entry.Type}] {entry.Message}", _labelStyle);
            GUI.contentColor = previousColor;
        }

        if (_logs.Count == 0)
            GUILayout.Label("수집된 로그 없음", _labelStyle);

        GUILayout.EndArea();
    }

    // 디버그창에 사용할 라벨/박스 스타일을 초기화(이미 스타일이 있으면 X)
    private void EnsureStyles()
    {
        if (_labelStyle != null && _boxStyle != null)
            return;

        _labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = LabelFontSize,
            wordWrap = false
        };

        _boxStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = BoxFontSize,
            alignment = TextAnchor.UpperCenter
        };
    }

    //로그가 들어갈 영역 계산
    private static Rect GetContentRect(Rect boxRect)
    {
        return new Rect(
            boxRect.x + BoxPaddingX,
            boxRect.y + BoxTitleHeight,
            boxRect.width - BoxPaddingX * 2f,
            boxRect.height - BoxTitleHeight - BoxPaddingX);
    }

    //프레임 업데이트
    private void UpdateFrameTime()
    {
        float currentDeltaTime = Time.unscaledDeltaTime;

        if (_smoothedDeltaTime <= 0f)
        {
            _smoothedDeltaTime = currentDeltaTime;
            return;
        }

        _smoothedDeltaTime = Mathf.Lerp(_smoothedDeltaTime, currentDeltaTime, FpsSmoothing);
    }

    //커서 활성화 여부
    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void HandleLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (_logs.Count >= MaxLogCount)
            _logs.RemoveAt(0);

        _logs.Add(new LogEntry(GetFirstLine(condition), type));
    }

    private static string GetFirstLine(string message)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;

        int lineBreakIndex = message.IndexOf('\n');
        string firstLine = lineBreakIndex >= 0 ? message.Substring(0, lineBreakIndex) : message;

        return firstLine.Length > MaxLogMessageLength
            ? firstLine.Substring(0, MaxLogMessageLength) + "..."
            : firstLine;
    }

    private static Color GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Warning:
                return Color.yellow;
            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                return Color.red;
            default:
                return Color.white;
        }
    }

    // 로그 구성
    private readonly struct LogEntry
    {
        public LogEntry(string message, LogType type)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; }
        public LogType Type { get; }
    }
}
#endif