using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private Button OptionButton;
    [SerializeField] private Button ExitButton;

    public void GameStart()
    {
        SceneManager.LoadScene("Mode Select Scene");
    }
    public void Option()
    {
        SceneManager.LoadScene("Option Scene");
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
