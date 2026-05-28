using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public GameDataSO gameData;

    public void OnPlayer1Select(int index) => gameData.p1CharacterIndex = index;
    public void OnPlayer2Select(int index) => gameData.p2CharacterIndex = index;

    public void OnStartGame(string mapName)
    {
        if (mapName == null) SceneManager.LoadScene("Map1");
        else SceneManager.LoadScene(mapName);
    }
}