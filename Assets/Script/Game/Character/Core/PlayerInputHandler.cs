using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController _playerController;
    private Vector2 _moveInput;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        _playerController.Move(_moveInput);
    }

    public void ResetInput()
    {
        _moveInput = Vector2.zero;
        _playerController.Move(Vector2.zero);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            _playerController.Jump();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            _playerController.Attack();
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.started)
            _playerController.Skill();
    }
}
