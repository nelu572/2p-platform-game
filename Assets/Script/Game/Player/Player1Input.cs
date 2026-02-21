using UnityEngine;

public class Player1Input : MonoBehaviour, IInputProvider
{
    public float GetMove()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.D)) move = 1f;
        return move;
    }

    public bool GetJump()
    {
        return Input.GetKeyDown(KeyCode.W);
    }
}