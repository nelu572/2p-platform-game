using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] private CharacterSelectData _characterSelectData;
    [SerializeField] private CharacterSelectUIController _characterSelectUIController;
    [SerializeField] private UIInput _uiInput;

    private void OnEnable()
    {
        _uiInput ??= FindFirstObjectByType<UIInput>();

        if (_uiInput != null)
            _uiInput.SubmitRequested += SaveCharacterSelection;
    }

    private void OnDisable()
    {
        if (_uiInput != null)
            _uiInput.SubmitRequested -= SaveCharacterSelection;
    }

    private bool SaveCharacterSelection(int playerIndex, UIButton button)
    {
        CharacterSelectButton selectButton = button.GetComponent<CharacterSelectButton>();
        if (selectButton == null)
            return _characterSelectUIController != null
                && _characterSelectUIController.OnSubmitRequested(playerIndex, button);

        if (playerIndex == 1)
            OnPlayer1Select(selectButton.CharacterIndex);
        else
            OnPlayer2Select(selectButton.CharacterIndex);

        return _characterSelectUIController != null
            && _characterSelectUIController.OnSubmitRequested(playerIndex, button);
    }

    public void OnPlayer1Select(int index) => _characterSelectData.p1CharacterIndex = index;
    public void OnPlayer2Select(int index) => _characterSelectData.p2CharacterIndex = index;

    public void OnStartGame(string mapName)
    {
        if (mapName == null) SceneManager.LoadScene("Map1");
        else SceneManager.LoadScene(mapName);
    }
}
