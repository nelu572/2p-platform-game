using UnityEngine;
using UnityEngine.SceneManagement;

public class GamaStart : MonoBehaviour
{
    [SerializeField] private CharacterSelectUI player1UI;
    [SerializeField] private CharacterSelectUI player2UI;

    public void GameStart()
    {
        CharacterSelectData.player1Index = player1UI.GetCharacter();
        CharacterSelectData.player2Index = player2UI.GetCharacter();

        SceneManager.LoadScene("Map1");
    }
}
