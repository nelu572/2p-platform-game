using UnityEngine;
using UnityEngine.SceneManagement;

public class ModSelect : MonoBehaviour
{
    public void BattleModStart()
    {
        SceneManager.LoadScene("Character Select Scene");
    }

    public void BossModStart()
    {
        SceneManager.LoadScene("Character Select Scene");
    }

}
