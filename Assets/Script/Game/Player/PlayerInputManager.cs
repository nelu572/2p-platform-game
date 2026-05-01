using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    PlayerMove playerMove;
    PlayerAction playerAction;

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAction = GetComponent<PlayerAction>();
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
        if (context.started)
            playerAction.Attack();
    }
    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.started)
            playerAction.Skill();
    }

}