using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 임시 코드:
/// 정식 Input System UI 구조를 붙이기 전까지 방향키/엔터로만
/// UIButton 이동과 실행을 테스트하기 위한 임시 네비게이터.
/// 나중에 Input System 기반 UI 입력 클래스로 교체 예정.
/// </summary>
public class UIKeyboardNavigator : MonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private UIButton _defaultButton;
    [SerializeField] private bool _autoSelectOnEnable = true;

    [Header("Repeat")]
    [SerializeField] private float _firstRepeatDelay = 0.25f;
    [SerializeField] private float _repeatInterval = 0.12f;

    private UIButton _currentButton;
    private Vector2 _lastMoveDirection;
    private float _nextMoveTime;

    private void OnEnable()
    {
        _currentButton = null;
        _lastMoveDirection = Vector2.zero;
        _nextMoveTime = 0f;

        if (_autoSelectOnEnable)
        {
            SelectButton(ResolveDefaultButton());
        }
    }

    private void Update()
    {
        EnsureSelection();
        HandleMove();
    }

    private void EnsureSelection()
    {
        if (IsSelectable(_currentButton))
        {
            return;
        }

        SelectButton(ResolveDefaultButton());
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

    private void LateUpdate()
    {
        if (_currentButton == null)
        {
            return;
        }

        // 임시 코드: 정식 Submit 액션 대신 Enter/Space로 클릭 처리.
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (EventSystem.current != null)
            {
                _currentButton.OnSubmit(new BaseEventData(EventSystem.current));
                return;
            }

            _currentButton.TriggerClick();
        }
    }

    private void MoveSelection(Vector2 moveDirection)
    {
        if (_currentButton == null)
        {
            SelectButton(ResolveDefaultButton());
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

    private UIButton ResolveDefaultButton()
    {
        if (IsSelectable(_defaultButton))
        {
            return _defaultButton;
        }

        UIButton[] buttons = GetComponentsInChildren<UIButton>(true);

        foreach (UIButton button in buttons)
        {
            if (IsSelectable(button))
            {
                return button;
            }
        }

        return null;
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
        // 임시 코드: 정식 Move 액션 대신 방향키/WASD 직접 폴링.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            return Vector2.left;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            return Vector2.right;
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            return Vector2.up;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            return Vector2.down;
        }

        return Vector2.zero;
    }
}
