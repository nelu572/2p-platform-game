using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    // 시작하자마자 열어 둘지 결정하는 기본 상태값.
    [SerializeField] private bool _baseEnabled = false;
    // 패널 표시와 입력 차단을 함께 제어한다.
    [SerializeField] private CanvasGroup _canvasGroup;
    // 패널이 어떤 방식으로 열리고 닫힐지 정의하는 설정값.
    [SerializeField] private PanelTransitionData _panelTransitionData = new PanelTransitionData();

    // 실제 애니메이션 실행은 TransitionPlayer 구현체에 위임한다.
    private IUITransitionPlayer _transitionPlayer;
    private Tween _activeTween;

    private void Awake()
    {
        EnsureComponents();
    }

    private void Start()
    {
        if (_baseEnabled) Open();
    }

    public void Open()
    {
        EnsureComponents();
        _activeTween?.Kill();

        // 닫혀 있던 패널은 먼저 켠 뒤에 Open 애니메이션을 재생한다.
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        SetInteraction(true);

        if (_transitionPlayer.CanPlay(_panelTransitionData.Type))
        {
            _transitionPlayer.Prepare(true);
            _activeTween = _transitionPlayer.CreateTween(true, _panelTransitionData);
            return;
        }

        _activeTween = null;
    }

    public void Close()
    {
        EnsureComponents();
        _activeTween?.Kill();

        // 닫히는 동안 중복 입력이 들어오지 않게 먼저 막는다.
        SetInteraction(false);

        if (_transitionPlayer.CanPlay(_panelTransitionData.Type))
        {
            _transitionPlayer.Prepare(false);
            _activeTween = _transitionPlayer.CreateTween(false, _panelTransitionData);
        }
        else
        {
            _activeTween = null;
        }

        if (_activeTween == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _activeTween.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void SetInteraction(bool isInteractable)
    {
        _canvasGroup.interactable = isInteractable;
        _canvasGroup.blocksRaycasts = isInteractable;
    }

    /// <summary>
    /// 패널 실행에 필요한 참조를 미리 보장한다.
    /// </summary>
    private void EnsureComponents()
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        if (_transitionPlayer == null)
        {
            // TODO: PanelType에 맞는 TransitionPlayer를 선택하도록 분기 추가
            _transitionPlayer = new UIPanelFadePlayer(_canvasGroup);
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 제거될 때 남아 있는 Tween도 같이 정리한다.
        _activeTween?.Kill();
    }
}
