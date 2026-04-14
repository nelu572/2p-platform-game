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

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJunp(InputAction.CallbackContext context)
    {
        _playerController.Jump();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _playerController.Attack();
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        _playerController.Skill();
    }
}
