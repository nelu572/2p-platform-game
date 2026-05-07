using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Input System 인스펙터 이벤트를 받아 UI 버튼 선택/실행만 처리한다.
/// 액션맵 연결과 입력 전환은 나중에 바깥 코드에서 관리할 수 있게
/// 이 클래스는 입력값 처리만 담당하도록 가볍게 유지한다.
/// </summary>
public class UIInput : MonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private UIButton _defaultButton;

    [Header("Repeat")]
    [SerializeField] private float _firstRepeatDelay = 0.25f;
    [SerializeField] private float _repeatInterval = 0.12f;

    private UIButton _currentButton;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection;
    private float _nextMoveTime;
    private bool _inputEnabled = true;

    void OnEnable()
    {
        _currentButton = null;
        _moveInput = Vector2.zero;
        _lastMoveDirection = Vector2.zero;
        _nextMoveTime = 0f;
        SelectDefaultButton();
    }

    void Update()
    {
        if (_inputEnabled == false)
        {
            return;
        }

        EnsureSelection();
        HandleMove();
    }

    // Input System 인스펙터의 Move(Vector2) 이벤트에 연결.
    public void OnMove(InputAction.CallbackContext context)
    {
        if (_inputEnabled == false)
        {
            return;
        }

        if (context.canceled)
        {
            _moveInput = Vector2.zero;
            _lastMoveDirection = Vector2.zero;
            return;
        }

        _moveInput = context.ReadValue<Vector2>();
    }

    // Input System 인스펙터의 Submit(Button) 이벤트에 연결.
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (_inputEnabled == false || context.started == false || _currentButton == null)
        {
            return;
        }

        if (EventSystem.current != null)
        {
            _currentButton.OnSubmit(new BaseEventData(EventSystem.current));
            return;
        }

        _currentButton.TriggerClick();
    }

    // Input System 인스펙터의 Cancel(Button) 이벤트에 연결.
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (_inputEnabled == false || context.started == false)
        {
            return;
        }

        // 나중에 외부 전환 코드나 팝업 닫기 로직을 붙일 자리.
    }

    public void EnableInput()
    {
        _inputEnabled = true;
        EnsureSelection();
    }

    public void DisableInput()
    {
        _inputEnabled = false;
        _moveInput = Vector2.zero;
        _lastMoveDirection = Vector2.zero;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SelectDefaultButton()
    {
        SelectButton(_defaultButton);
    }

    private void EnsureSelection()
    {
        if (IsSelectable(_currentButton))
        {
            return;
        }

        SelectDefaultButton();
    }

    private void HandleMove()
    {
        Vector2 moveDirection = ReadMoveDirection();

        if (moveDirection == Vector2.zero)
        {
            _lastMoveDirection = Vector2.zero;
            return;
        }

        bool isNewDirection = moveDirection != _lastMoveDirection;
        bool canRepeat = Time.unscaledTime >= _nextMoveTime;

        if (isNewDirection || canRepeat)
        {
            MoveSelection(moveDirection);
            _lastMoveDirection = moveDirection;
            _nextMoveTime = Time.unscaledTime + (isNewDirection ? _firstRepeatDelay : _repeatInterval);
        }
    }

    private void MoveSelection(Vector2 moveDirection)
    {
        if (_currentButton == null)
        {
            SelectDefaultButton();
            return;
        }

        UIButton nextButton = _currentButton.GetMoveTarget(moveDirection);

        if (IsSelectable(nextButton))
        {
            SelectButton(nextButton);
        }
    }

    private void SelectButton(UIButton button)
    {
        if (IsSelectable(button) == false)
        {
            return;
        }

        _currentButton = button;
        _currentButton.Select();
    }

    private bool IsSelectable(UIButton button)
    {
        return button != null
            && button.isActiveAndEnabled
            && button.IsInteractable()
            && button.gameObject.activeInHierarchy;
    }

    private Vector2 ReadMoveDirection()
    {
        if (_moveInput.sqrMagnitude <= 0.01f)
        {
            return Vector2.zero;
        }

        if (Mathf.Abs(_moveInput.x) > Mathf.Abs(_moveInput.y))
        {
            return _moveInput.x > 0f ? Vector2.right : Vector2.left;
        }

        return _moveInput.y > 0f ? Vector2.up : Vector2.down;
    }
}
