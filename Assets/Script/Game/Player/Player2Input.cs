using UnityEngine;

public class Player2Input : MonoBehaviour, IInputProvider
{
    public float GetMove()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) move = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) move = 1f;
        return move;
    }

    public bool GetJump()
    {
        return Input.GetKeyDown(KeyCode.UpArrow);
    }
}
