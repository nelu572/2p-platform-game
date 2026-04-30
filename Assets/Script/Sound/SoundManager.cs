using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float _bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (_bgmSource != null)
            _bgmSource.volume = _bgmVolume;

        if (_sfxSource != null)
            _sfxSource.volume = _sfxVolume;
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || _bgmSource == null) return;

        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource == null) return;
        _bgmSource.Stop();
    }

    public void SFXPlay(AudioClip clip)
    {
        if (clip == null || _sfxSource == null) return;

        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = volume;
        if (_bgmSource != null)
            _bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
    }
}