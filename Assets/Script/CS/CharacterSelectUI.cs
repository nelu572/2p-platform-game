using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Animator imgAnima;
    [SerializeField] private int playerNumber; // 1 or 2

    private int currentCharacter;

    public void SelectCharacter(int index)
    {
        currentCharacter = index;
        imgAnima.SetInteger("AnimaNum", index);
    }

    public int GetCharacter()
    {
        return currentCharacter;
    }

    public int GetPlayerNumber()
    {
        return playerNumber;
    }
}