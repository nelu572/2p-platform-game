using UnityEngine;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private int _characterIndex;

    public int CharacterIndex => _characterIndex;
}
