using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class CharacterSelectData : ScriptableObject
{
    public int p1CharacterIndex = 0;
    public int p2CharacterIndex = 1;

    [Header("캐릭터 프리팹")]
    public GameObject[] characterPrefabs; // 캐릭터 프리팹을 넣을 배열
}