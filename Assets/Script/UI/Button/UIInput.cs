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
    public delegate bool PlayerButtonRequestHandler(int playerIndex, UIButton button);
    public event PlayerButtonRequestHandler SubmitRequested;
    public event PlayerButtonRequestHandler CancelRequested;
    public event System.Action<int, UIButton> SelectionChanged;

    [Header("Repeat")]
    [SerializeField] private float _firstRepeatDelay = 0.25f;
    [SerializeField] private float _repeatInterval = 0.12f;

    // UIInpu의 중복 생성을 막기위한 필드
    private static UIInput _persistentInstance;

    private readonly PlayerCursor _player1 = new PlayerCursor();
    private readonly PlayerCursor _player2 = new PlayerCursor();

    private UIButton _defaultButton;
    private bool _inputEnabled = true;

    void Awake()
    {
        if (_persistentInstance != null && _persistentInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        _persistentInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        ResetCursor(_player1);
        ResetCursor(_player2);
        SelectDefaultButton();
    }

    void Update()
    {
        if (_inputEnabled == false)
        {
            return;
        }

        EnsureSelection(_player1);
        EnsureSelection(_player2);
        HandleMove(_player1);
        HandleMove(_player2);
    }

    // Input System 인스펙터의 Move(Vector2) 이벤트에 연결.
    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerCursor cursor = GetCursor(context);
        cursor.ReadMove(context, IsCursorEnabled(cursor));
    }

    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        _player1.ReadMove(context, IsCursorEnabled(_player1));
    }

    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        _player2.ReadMove(context, IsCursorEnabled(_player2));
    }

    // Input System 인스펙터의 Submit(Button) 이벤트에 연결.
    public void OnSubmit(InputAction.CallbackContext context)
    {
        Submit(GetCursor(context), context);
    }

    public void OnPlayer1Submit(InputAction.CallbackContext context)
    {
        Submit(_player1, context);
    }

    public void OnPlayer2Submit(InputAction.CallbackContext context)
    {
        Submit(_player2, context);
    }

    // Input System 인스펙터의 Cancel(Button) 이벤트에 연결.
    public void OnCancel(InputAction.CallbackContext context)
    {
        Cancel(GetCursor(context), context);
    }

    public void OnPlayer1Cancel(InputAction.CallbackContext context)
    {
        Cancel(_player1, context);
    }

    public void OnPlayer2Cancel(InputAction.CallbackContext context)
    {
        Cancel(_player2, context);
    }

    public void SetDefaultButton(UIButton defaultButton)
    {
        _defaultButton = defaultButton;
        SelectDefaultButton();
    }

    public void EnableInput()
    {
        _inputEnabled = true;
        EnsureSelection(_player1);
        EnsureSelection(_player2);
    }

    public void DisableInput()
    {
        _inputEnabled = false;
        ResetCursorInput(_player1);
        ResetCursorInput(_player2);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SetPlayer1InputEnabled(bool enabled)
    {
        SetCursorInputEnabled(_player1, enabled);
    }

    public void EnablePlayer1Input()
    {
        SetPlayer1InputEnabled(true);
    }

    public void DisablePlayer1Input()
    {
        SetPlayer1InputEnabled(false);
    }

    public void SetPlayer2InputEnabled(bool enabled)
    {
        SetCursorInputEnabled(_player2, enabled);
    }

    public void EnablePlayer2Input()
    {
        SetPlayer2InputEnabled(true);
    }

    public void DisablePlayer2Input()
    {
        SetPlayer2InputEnabled(false);
    }

    public void SetOnlyPlayer1InputEnabled(bool enabled)
    {
        SetPlayer1InputEnabled(enabled);
        SetPlayer2InputEnabled(false);
    }

    public void EnableOnlyPlayer1Input()
    {
        SetOnlyPlayer1InputEnabled(true);
    }

    public void SelectDefaultButton()
    {
        SelectButton(_player1, _defaultButton);
        SelectButton(_player2, _defaultButton);
    }

    public UIButton GetCurrentButton()
    {
        return _player1.CurrentButton;
    }

    public UIButton GetPlayer1CurrentButton()
    {
        return _player1.CurrentButton;
    }

    public UIButton GetPlayer2CurrentButton()
    {
        return _player2.CurrentButton;
    }

    public void SetPlayer1CurrentButton(UIButton button)
    {
        SelectButton(_player1, button);
    }

    public void SetPlayer2CurrentButton(UIButton button)
    {
        SelectButton(_player2, button);
    }

    public void SetPlayerCurrentButton(int playerIndex, UIButton button)
    {
        SelectButton(GetCursor(playerIndex), button);
    }

    private void Submit(PlayerCursor cursor, InputAction.CallbackContext context)
    {
        if (IsCursorEnabled(cursor) == false || context.started == false || cursor.CurrentButton == null)
        {
            return;
        }

        if (InvokeRequest(SubmitRequested, GetPlayerIndex(cursor), cursor.CurrentButton))
        {
            return;
        }

        if (cursor == _player1 && EventSystem.current != null)
        {
            cursor.CurrentButton.OnSubmit(new BaseEventData(EventSystem.current));
            return;
        }

        cursor.CurrentButton.TriggerClick();
    }

    private void Cancel(PlayerCursor cursor, InputAction.CallbackContext context)
    {
        if (IsCursorEnabled(cursor) == false || context.started == false)
        {
            return;
        }

        if (InvokeRequest(CancelRequested, GetPlayerIndex(cursor), cursor.CurrentButton))
        {
            return;
        }

        // 나중에 외부 전환 코드나 팝업 닫기 로직을 붙일 자리.
    }

    private void SetCursorInputEnabled(PlayerCursor cursor, bool enabled)
    {
        cursor.InputEnabled = enabled;

        if (enabled == false)
        {
            ResetCursorInput(cursor);
        }
        else
        {
            EnsureSelection(cursor);
        }
    }

    private bool IsCursorEnabled(PlayerCursor cursor)
    {
        return _inputEnabled && cursor.InputEnabled;
    }

    private void ResetCursor(PlayerCursor cursor)
    {
        cursor.CurrentButton = null;
        cursor.InputEnabled = true;
        ResetCursorInput(cursor);
    }

    private void ResetCursorInput(PlayerCursor cursor)
    {
        cursor.MoveInput = Vector2.zero;
        cursor.LastMoveDirection = Vector2.zero;
        cursor.NextMoveTime = 0f;
    }

    private void EnsureSelection(PlayerCursor cursor)
    {
        if (IsSelectable(cursor.CurrentButton))
        {
            return;
        }

        SelectButton(cursor, _defaultButton);
    }

    private void HandleMove(PlayerCursor cursor)
    {
        if (IsCursorEnabled(cursor) == false)
        {
            return;
        }

        Vector2 moveDirection = ReadMoveDirection(cursor.MoveInput);

        if (moveDirection == Vector2.zero)
        {
            cursor.LastMoveDirection = Vector2.zero;
            return;
        }

        bool isNewDirection = moveDirection != cursor.LastMoveDirection;
        bool canRepeat = Time.unscaledTime >= cursor.NextMoveTime;

        if (isNewDirection || canRepeat)
        {
            MoveSelection(cursor, moveDirection);
            cursor.LastMoveDirection = moveDirection;
            cursor.NextMoveTime = Time.unscaledTime + (isNewDirection ? _firstRepeatDelay : _repeatInterval);
        }
    }

    private void MoveSelection(PlayerCursor cursor, Vector2 moveDirection)
    {
        if (cursor.CurrentButton == null)
        {
            SelectButton(cursor, _defaultButton);
            return;
        }

        UIButton nextButton = cursor.CurrentButton.GetMoveTarget(moveDirection);

        if (IsSelectable(nextButton))
        {
            SelectButton(cursor, nextButton);
        }
    }

    private void SelectButton(PlayerCursor cursor, UIButton button)
    {
        if (IsSelectable(button) == false)
        {
            return;
        }

        cursor.CurrentButton = button;
        SelectionChanged?.Invoke(GetPlayerIndex(cursor), button);

        if (cursor == _player1 && EventSystem.current != null)
        {
            button.Select();
        }
        else
        {
            button.TriggerHover();
        }
    }

    private bool IsSelectable(UIButton button)
    {
        return button != null
            && button.isActiveAndEnabled
            && button.IsInteractable()
            && button.gameObject.activeInHierarchy;
    }

    private Vector2 ReadMoveDirection(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude <= 0.01f)
        {
            return Vector2.zero;
        }

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            return moveInput.x > 0f ? Vector2.right : Vector2.left;
        }

        return moveInput.y > 0f ? Vector2.up : Vector2.down;
    }

    private PlayerCursor GetCursor(InputAction.CallbackContext context)
    {
        string actionMapName = context.action?.actionMap?.name;
        string actionName = context.action?.name;

        if (IsPlayer2Action(actionMapName) || IsPlayer2Action(actionName))
        {
            return _player2;
        }

        return _player1;
    }

    private PlayerCursor GetCursor(int playerIndex)
    {
        return playerIndex == 2 ? _player2 : _player1;
    }

    private int GetPlayerIndex(PlayerCursor cursor)
    {
        return cursor == _player2 ? 2 : 1;
    }

    private bool IsPlayer2Action(string actionName)
    {
        return string.IsNullOrEmpty(actionName) == false
            && (actionName.Contains("Player2") || actionName.Contains("P2"));
    }

    private bool InvokeRequest(PlayerButtonRequestHandler handler, int playerIndex, UIButton button)
    {
        if (handler == null)
        {
            return false;
        }

        foreach (PlayerButtonRequestHandler callback in handler.GetInvocationList())
        {
            if (callback.Invoke(playerIndex, button))
            {
                return true;
            }
        }

        return false;
    }

    private sealed class PlayerCursor
    {
        public UIButton CurrentButton;
        public Vector2 MoveInput;
        public Vector2 LastMoveDirection;
        public float NextMoveTime;
        public bool InputEnabled = true;

        public void ReadMove(InputAction.CallbackContext context, bool inputEnabled)
        {
            if (inputEnabled == false)
            {
                MoveInput = Vector2.zero;
                LastMoveDirection = Vector2.zero;
                return;
            }

            if (context.canceled)
            {
                MoveInput = Vector2.zero;
                LastMoveDirection = Vector2.zero;
                return;
            }

            MoveInput = context.ReadValue<Vector2>();
        }
    }
}
