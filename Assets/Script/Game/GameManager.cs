using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private CameraMove cameraMove;

    void Start()
    {
        CharacterSelectData.player1Index = 0;
        CharacterSelectData.player2Index = 0;

        GameObject p1 = SpawnPlayer(CharacterSelectData.player1Index, "Player1", new Vector3(-3, 0, 0));
        GameObject p2 = SpawnPlayer(CharacterSelectData.player2Index, "Player2", new Vector3(-3, 0, 0));


        cameraMove.SetTargets(p1.transform, p2.transform);

    }

    GameObject SpawnPlayer(int index, string actionMap, Vector3 pos)
    {
        GameObject player = Instantiate(characterPrefabs[index], pos, Quaternion.identity);

        PlayerInput input = player.GetComponent<PlayerInput>();
        input.SwitchCurrentActionMap(actionMap);

        return player;
    }
}