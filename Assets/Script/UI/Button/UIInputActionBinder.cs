using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// UI 전용 액션맵을 UIInput에 코드로 연결한다.
/// PlayerInput의 인스펙터 이벤트를 플레이어 전투 입력과 공유하지 않기 위한 보조 바인더다.
/// </summary>
[RequireComponent(typeof(UIInput))]
public class UIInputActionBinder : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    private UIInput _uiInput;
    private InputAction _p1Move;
    private InputAction _p1Submit;
    private InputAction _p1Cancel;
    private InputAction _p2Move;
    private InputAction _p2Submit;
    private InputAction _p2Cancel;

    private void Awake()
    {
        _uiInput = GetComponent<UIInput>();
        _playerInput ??= GetComponent<PlayerInput>();
        CacheActions();
    }

    private void OnEnable()
    {
        BindActions();
        SetActionMapsEnabled(true);
    }

    private void OnDisable()
    {
        SetActionMapsEnabled(false);
        UnbindActions();
    }

    private void CacheActions()
    {
        InputActionAsset actions = _playerInput != null ? _playerInput.actions : null;

        if (actions == null)
        {
            return;
        }

        _p1Move = actions.FindAction("UI_P1/Move", false);
        _p1Submit = actions.FindAction("UI_P1/Submit", false);
        _p1Cancel = actions.FindAction("UI_P1/Cancel", false);
        _p2Move = actions.FindAction("UI_P2/Move", false);
        _p2Submit = actions.FindAction("UI_P2/Submit", false);
        _p2Cancel = actions.FindAction("UI_P2/Cancel", false);
    }

    private void BindActions()
    {
        BindMove(_p1Move, _uiInput.OnPlayer1Move);
        BindButton(_p1Submit, _uiInput.OnPlayer1Submit);
        BindButton(_p1Cancel, _uiInput.OnPlayer1Cancel);
        BindMove(_p2Move, _uiInput.OnPlayer2Move);
        BindButton(_p2Submit, _uiInput.OnPlayer2Submit);
        BindButton(_p2Cancel, _uiInput.OnPlayer2Cancel);
    }

    private void UnbindActions()
    {
        UnbindMove(_p1Move, _uiInput.OnPlayer1Move);
        UnbindButton(_p1Submit, _uiInput.OnPlayer1Submit);
        UnbindButton(_p1Cancel, _uiInput.OnPlayer1Cancel);
        UnbindMove(_p2Move, _uiInput.OnPlayer2Move);
        UnbindButton(_p2Submit, _uiInput.OnPlayer2Submit);
        UnbindButton(_p2Cancel, _uiInput.OnPlayer2Cancel);
    }

    private void SetActionMapsEnabled(bool enabled)
    {
        SetActionMapEnabled(_p1Move, enabled);
        SetActionMapEnabled(_p2Move, enabled);
    }

    private void SetActionMapEnabled(InputAction action, bool enabled)
    {
        if (action?.actionMap == null)
        {
            return;
        }

        if (enabled)
        {
            action.actionMap.Enable();
        }
        else
        {
            action.actionMap.Disable();
        }
    }

    private void BindMove(InputAction action, System.Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            return;
        }

        action.performed += callback;
        action.canceled += callback;
    }

    private void UnbindMove(InputAction action, System.Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            return;
        }

        action.performed -= callback;
        action.canceled -= callback;
    }

    private void BindButton(InputAction action, System.Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            return;
        }

        action.started += callback;
    }

    private void UnbindButton(InputAction action, System.Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            return;
        }

        action.started -= callback;
    }
}
