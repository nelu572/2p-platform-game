
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    PlayerMove playerMove;

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMove.SetMoveInput(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) playerMove.SetIsJumpHeld(true);
        if (context.canceled) playerMove.SetIsJumpHeld(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

    }
}