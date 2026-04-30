using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _bgmClip;

    void Start()
    {
        SoundManager.Instance.PlayBGM(_bgmClip);
    }
}