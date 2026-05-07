using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIButton : Button
{
    [Header("Sound")]
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _hoverSound;

    [Header("Button Logic")]
    [SerializeField] private ButtonType _buttonType;

    [SerializeField] private UIPanel _enablePanel;
    [SerializeField] private UIPanel _disablePanel;

    [SerializeField] private string _nextSceneName;

    [Header("Directional Navigation")]
    [SerializeField] private UIButton _upButton;
    [SerializeField] private UIButton _downButton;
    [SerializeField] private UIButton _leftButton;
    [SerializeField] private UIButton _rightButton;

    private SoundManager _soundManager;
    private bool _isHoverSoundReady = true;

    protected override void Start()
    {
        base.Start();
        _soundManager = SoundManager.Instance;
    }

    public UIButton UpButton => _upButton;
    public UIButton DownButton => _downButton;
    public UIButton LeftButton => _leftButton;
    public UIButton RightButton => _rightButton;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        TriggerClick();
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        TriggerClick();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        TriggerHover();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _isHoverSoundReady = true;
    }

    public UIButton GetMoveTarget(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0f ? _rightButton : _leftButton;
        }

        if (Mathf.Abs(direction.y) > 0f)
        {
            return direction.y > 0f ? _upButton : _downButton;
        }

        return null;
    }

    public void TriggerHover()
    {
        if (IsInteractable() == false)
        {
            return;
        }

        if (_hoverSound != null && _isHoverSoundReady)
        {
            _soundManager?.SFXPlay(_hoverSound);
        }

        _isHoverSoundReady = false;
    }

    public void TriggerClick()
    {
        if (IsActive() == false || IsInteractable() == false)
        {
            return;
        }

        HandleClick();
    }

    private void HandleClick()
    {
        // 사운드
        if (_clickSound != null)
            _soundManager.SFXPlay(_clickSound);

        // 타입별 동작
        switch (_buttonType)
        {
            case ButtonType.ChangePanel:
                ChangeObject();
                break;

            case ButtonType.OpenPopup:
                if (_enablePanel != null)
                    _enablePanel.Open();
                break;

            case ButtonType.ClosePopup:
                if (_disablePanel != null)
                    _disablePanel.Close();
                break;

            case ButtonType.GoScene:
                SceneManager.LoadScene(_nextSceneName);
                break;

            case ButtonType.Quit:
                QuitGame();
                break;
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ChangeObject()
    {
        if (_disablePanel != null)
            _disablePanel.Close();

        if (_enablePanel != null)
            _enablePanel.Open();
    }
}
