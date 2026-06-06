using UnityEngine;
using UnityEngine.InputSystem;

public class ChargeInputHandler : MonoBehaviour
{
    private IChargeable _chargeable;
    private PlayerInput _playerInput;

    void Awake()
    {
        _chargeable = GetComponent<IChargeable>();
        _playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        if (_chargeable == null || _playerInput == null) return;

        // IChargeable을 구현한 경우에만 모든 액션 canceled 구독
        foreach (var action in _playerInput.actions)
            action.canceled += OnActionCanceled;
    }

    void OnDisable()
    {
        if (_playerInput == null) return;

        foreach (var action in _playerInput.actions)
            action.canceled -= OnActionCanceled;
    }

    private void OnActionCanceled(InputAction.CallbackContext ctx)
    {
        _chargeable?.ReleaseCharge(ctx.action.name);
    }
}
