using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO: 지금은 그냥 반투명 어두운 이미지만 생성되지만 나중에는 왼쪽에 다시하기 종료하기 같은 버튼과 
/// 전투의 결과와 피해량등 전투 관련 결과를 보여주는 창이 필요하고 이를 등장하는 UI에니메이션 같은게 필요합니다
/// </summary>
public class RoundEndUI : MonoBehaviour
{
    //비워두도 됨
    [SerializeField] private GameObject _root;
    [SerializeField] private Image _blackOverlayImage;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private byte _targetAlpha = 180;

    private Coroutine _fadeCoroutine;
    private bool _isShowing;

    private void Awake()
    {
        CacheReferences();
    }

    private void Start()
    {
        if (!_isShowing)
            HideImmediate();
    }

    public void Show()
    {
        CacheReferences();
        _isShowing = true;

        if (_blackOverlayImage == null)
            return;

        if (_root != null && !_root.activeSelf)
            _root.SetActive(true);

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeIn());
    }

    public void HideImmediate()
    {
        _isShowing = false;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        SetOverlayAlpha(0f);

        if (_root != null)
            _root.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float targetAlpha = _targetAlpha / 255f;

        SetOverlayAlpha(0f);

        if (_fadeDuration <= 0f)
        {
            SetOverlayAlpha(targetAlpha);
            _fadeCoroutine = null;
            yield break;
        }

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _fadeDuration);
            SetOverlayAlpha(Mathf.Lerp(0f, targetAlpha, progress));
            yield return null;
        }

        SetOverlayAlpha(targetAlpha);
        _fadeCoroutine = null;
    }

    private void SetOverlayAlpha(float alpha)
    {
        CacheReferences();

        if (_blackOverlayImage == null)
            return;

        _blackOverlayImage.color = new Color(0f, 0f, 0f, alpha);
    }

    private void CacheReferences()
    {
        if (_root == null)
            _root = gameObject;

        if (_blackOverlayImage == null)
            _blackOverlayImage = GetComponentInChildren<Image>(true);
    }
}
