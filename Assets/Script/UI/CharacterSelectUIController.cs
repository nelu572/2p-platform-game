using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 캐릭터 선택 씬에서 P1/P2의 hover 표시와 확인/취소 이동만 처리한다.
/// 실제 캐릭터 선택 데이터 저장은 추후 선택 로직에서 연결한다.
/// </summary>
public class CharacterSelectUIController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private UIInput _uiInput;

    [Header("Buttons")]
    [SerializeField] private UIButton _defaultCharacterButton;
    [SerializeField] private UIButton _p1PanelButton;
    [SerializeField] private UIButton _p2PanelButton;

    [Header("Hover")]
    [SerializeField] private Color _p1HoverColor = new Color(0.1f, 0.35f, 1f, 0.95f);
    [SerializeField] private Color _p2HoverColor = new Color(1f, 0.12f, 0.12f, 0.95f);
    [SerializeField] private float _hoverThickness = 8f;
    [SerializeField] private float _p1Padding = 6f;
    [SerializeField] private float _p2Padding = 16f;

    [Header("Confirm")]
    [SerializeField] private Color _confirmedColor = new Color(1f, 0.85f, 0.05f, 1f);
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private int _countdownSeconds = 5;
    [SerializeField] private string _nextSceneName = "Map1";

    private readonly Dictionary<UIButton, Color> _originalButtonColors = new Dictionary<UIButton, Color>();
    private UIButton _p1SelectedCharacterButton;
    private UIButton _p2SelectedCharacterButton;
    private HoverMarker _p1Hover;
    private HoverMarker _p2Hover;
    private bool _isInputBound;
    private Coroutine _countdownRoutine;

    private void Awake()
    {
        BindSceneReferences();
        _p1Hover = HoverMarker.Create(transform, "P1 Hover", _p1HoverColor, _hoverThickness, _p1Padding);
        _p2Hover = HoverMarker.Create(transform, "P2 Hover", _p2HoverColor, _hoverThickness, _p2Padding);
    }

    private void OnEnable()
    {
        TryBindInput();
    }

    private void Start()
    {
        TryBindInput();
    }

    private void OnDisable()
    {
        StopCountdown();

        if (_uiInput == null)
        {
            return;
        }

        _uiInput.SubmitRequested -= OnSubmitRequested;
        _uiInput.CancelRequested -= OnCancelRequested;
        _uiInput.SelectionChanged -= OnSelectionChanged;
        _isInputBound = false;
    }

    private void Update()
    {
        if (_uiInput == null)
        {
            TryBindInput();
            return;
        }

        _p1Hover.MoveTo(_uiInput.GetPlayer1CurrentButton());
        _p2Hover.MoveTo(_uiInput.GetPlayer2CurrentButton());
    }

    private void BindSceneReferences()
    {
        _defaultCharacterButton ??= FindDefaultCharacterButton();
        _p1PanelButton ??= FindOrCreatePanelButton("P1Panel");
        _p2PanelButton ??= FindOrCreatePanelButton("P2Panel");
        _countdownText ??= FindChild("LeftTime")?.GetComponent<TMP_Text>();

        if (_countdownText != null)
        {
            _countdownText.gameObject.SetActive(false);
        }
    }

    private void TryBindInput()
    {
        UIInput nextInput = _uiInput != null ? _uiInput : FindFirstObjectByType<UIInput>();

        if (nextInput == null)
        {
            return;
        }

        if (_uiInput == nextInput && _isInputBound)
        {
            return;
        }

        if (_uiInput != null && _isInputBound)
        {
            _uiInput.SubmitRequested -= OnSubmitRequested;
            _uiInput.CancelRequested -= OnCancelRequested;
            _uiInput.SelectionChanged -= OnSelectionChanged;
        }

        _uiInput = nextInput;
        _uiInput.SetDefaultButton(_defaultCharacterButton);
        _uiInput.SetPlayer1CurrentButton(_defaultCharacterButton);
        _uiInput.SetPlayer2CurrentButton(_defaultCharacterButton);
        _uiInput.SubmitRequested += OnSubmitRequested;
        _uiInput.CancelRequested += OnCancelRequested;
        _uiInput.SelectionChanged += OnSelectionChanged;
        _isInputBound = true;
    }

    private bool OnSubmitRequested(int playerIndex, UIButton button)
    {
        if (IsCharacterButton(button) == false)
        {
            return IsPanelButton(button);
        }

        if (playerIndex == 1)
        {
            _p1SelectedCharacterButton = button;
            SetConfirmedColor(button);
            _uiInput.SetPlayer1CurrentButton(_p1PanelButton);
        }
        else
        {
            _p2SelectedCharacterButton = button;
            SetConfirmedColor(button);
            _uiInput.SetPlayer2CurrentButton(_p2PanelButton);
        }

        TryStartCountdown();
        return true;
    }

    private bool OnCancelRequested(int playerIndex, UIButton button)
    {
        UIButton selectedCharacterButton = playerIndex == 1 ? _p1SelectedCharacterButton : _p2SelectedCharacterButton;

        if (selectedCharacterButton == null)
        {
            return false;
        }

        if (playerIndex == 1)
        {
            _p1SelectedCharacterButton = null;
            RestoreButtonColorIfUnused(selectedCharacterButton);
            _uiInput.SetPlayer1CurrentButton(selectedCharacterButton);
        }
        else
        {
            _p2SelectedCharacterButton = null;
            RestoreButtonColorIfUnused(selectedCharacterButton);
            _uiInput.SetPlayer2CurrentButton(selectedCharacterButton);
        }

        StopCountdown();
        return true;
    }

    private void OnSelectionChanged(int playerIndex, UIButton button)
    {
        if (playerIndex == 1)
        {
            _p1Hover.MoveTo(button);
        }
        else
        {
            _p2Hover.MoveTo(button);
        }
    }

    private UIButton FindDefaultCharacterButton()
    {
        foreach (UIButton button in FindObjectsByType<UIButton>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID))
        {
            if (IsCharacterButton(button))
            {
                return button;
            }
        }

        return null;
    }

    private UIButton FindOrCreatePanelButton(string panelName)
    {
        Transform panel = FindChild(panelName);

        if (panel == null)
        {
            return null;
        }

        Transform buttonTransform = FindChild(panel, "Button");

        if (buttonTransform == null)
        {
            buttonTransform = panel;
        }

        UIButton button = buttonTransform.GetComponent<UIButton>();

        if (button != null)
        {
            return button;
        }

        button = buttonTransform.gameObject.AddComponent<UIButton>();
        Image image = buttonTransform.GetComponent<Image>();

        if (image != null)
        {
            button.targetGraphic = image;
        }

        return button;
    }

    private Transform FindChild(string childName)
    {
        return FindChild(transform, childName);
    }

    private Transform FindChild(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    private bool IsCharacterButton(UIButton button)
    {
        return button != null && IsPanelButton(button) == false && IsInPlayerPanel(button.transform) == false;
    }

    private bool IsPanelButton(UIButton button)
    {
        return button != null && (button == _p1PanelButton || button == _p2PanelButton);
    }

    private bool IsInPlayerPanel(Transform target)
    {
        while (target != null)
        {
            if (target.name == "P1Panel" || target.name == "P2Panel")
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }

    private void SetConfirmedColor(UIButton button)
    {
        Graphic targetGraphic = button.targetGraphic;

        if (targetGraphic == null)
        {
            return;
        }

        if (_originalButtonColors.ContainsKey(button) == false)
        {
            _originalButtonColors.Add(button, targetGraphic.color);
        }

        targetGraphic.color = _confirmedColor;
    }

    private void RestoreButtonColorIfUnused(UIButton button)
    {
        if (button == null || _p1SelectedCharacterButton == button || _p2SelectedCharacterButton == button)
        {
            return;
        }

        if (_originalButtonColors.TryGetValue(button, out Color originalColor) == false)
        {
            return;
        }

        if (button.targetGraphic != null)
        {
            button.targetGraphic.color = originalColor;
        }

        _originalButtonColors.Remove(button);
    }

    private void TryStartCountdown()
    {
        if (_p1SelectedCharacterButton == null || _p2SelectedCharacterButton == null || _countdownRoutine != null)
        {
            return;
        }

        _countdownRoutine = StartCoroutine(CountdownAndLoadScene());
    }

    private void StopCountdown()
    {
        if (_countdownRoutine != null)
        {
            StopCoroutine(_countdownRoutine);
            _countdownRoutine = null;
        }

        if (_countdownText != null)
        {
            _countdownText.gameObject.SetActive(false);
        }
    }

    private IEnumerator CountdownAndLoadScene()
    {
        if (_countdownText != null)
        {
            _countdownText.gameObject.SetActive(true);
        }

        for (int leftTime = _countdownSeconds; leftTime > 0; leftTime--)
        {
            if (_countdownText != null)
            {
                _countdownText.text = leftTime.ToString("00");
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        _countdownRoutine = null;

        if (string.IsNullOrWhiteSpace(_nextSceneName))
        {
            Debug.LogWarning("캐릭터 선택 후 이동할 씬 이름이 비어 있습니다.", this);
            yield break;
        }

        SceneManager.LoadScene(_nextSceneName);
    }

    private sealed class HoverMarker
    {
        private readonly RectTransform _root;
        private readonly float _padding;

        private HoverMarker(RectTransform root, float padding)
        {
            _root = root;
            _padding = padding;
        }

        public static HoverMarker Create(Transform parent, string name, Color color, float thickness, float padding)
        {
            RectTransform root = new GameObject(name).AddComponent<RectTransform>();
            root.SetParent(parent, false);

            CreateLine(root, "Top", color, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, -padding), new Vector2(0f, thickness));
            CreateLine(root, "Bottom", color, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, padding), new Vector2(0f, thickness));
            CreateLine(root, "Left", color, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(-padding, 0f), new Vector2(thickness, 0f));
            CreateLine(root, "Right", color, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(padding, 0f), new Vector2(thickness, 0f));

            return new HoverMarker(root, padding);
        }

        public void MoveTo(UIButton button)
        {
            if (button == null)
            {
                _root.gameObject.SetActive(false);
                return;
            }

            _root.gameObject.SetActive(true);
            _root.SetParent(button.transform, false);
            _root.SetAsLastSibling();
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.offsetMin = new Vector2(-_padding, -_padding);
            _root.offsetMax = new Vector2(_padding, _padding);
        }

        private static void CreateLine(RectTransform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            Image image = new GameObject(name).AddComponent<Image>();
            image.raycastTarget = false;
            image.color = color;

            RectTransform rectTransform = image.rectTransform;
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }
    }
}
