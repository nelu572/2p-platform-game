using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    // 시작하자마자 열어 둘지 결정하는 기본 상태값.
    [SerializeField] private bool _baseEnabled = false;
    // 패널 표시와 입력 차단을 함께 제어한다.
    [SerializeField] private CanvasGroup _canvasGroup;
    // Scale 전환에서 시작/종료 크기를 제어한다.
    [SerializeField] private RectTransform _rectTransform;
    // 패널이 어떤 방식으로 열리고 닫힐지 정의하는 설정값.
    [SerializeField] private PanelTransitionData _panelTransitionData = new PanelTransitionData();

    // 실제 애니메이션 실행은 TransitionPlayer 구현체 목록에 위임한다.
    private IUITransitionPlayer[] _transitionPlayers;
    private Tween _activeTween;

    void Awake()
    {
        EnsureComponents();
    }

    void Start()
    {
        if (_baseEnabled)
            Open();
    }

    public void Open()
    {
        EnsureComponents();
        _activeTween?.Kill();

        // 닫혀 있던 패널은 먼저 켠 뒤에 Open 애니메이션을 재생한다.
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        SetInteraction(true);
        _activeTween = CreateTransitionTween(true);
    }

    public void Close()
    {
        EnsureComponents();
        _activeTween?.Kill();

        // 닫히는 동안 중복 입력이 들어오지 않게 먼저 막는다.
        SetInteraction(false);
        _activeTween = CreateTransitionTween(false);

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
    /// 현재 PanelType에 맞는 TransitionPlayer만 골라서 하나의 Tween으로 묶는다.
    /// </summary>
    private Tween CreateTransitionTween(bool isOpening)
    {
        Sequence sequence = null;

        foreach (IUITransitionPlayer transitionPlayer in _transitionPlayers)
        {
            if (transitionPlayer.CanPlay(_panelTransitionData.Type) == false)
            {
                continue;
            }

            // 각 플레이어가 자기 타입의 시작 상태를 먼저 맞춘다.
            transitionPlayer.Prepare(isOpening, _panelTransitionData);

            if (sequence == null)
            {
                sequence = DOTween.Sequence();
            }

            sequence.Join(transitionPlayer.CreateTween(isOpening, _panelTransitionData));
        }

        return sequence;
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

        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        if (_transitionPlayers == null)
        {
            // Move까지 포함해서 패널 타입별 플레이어를 한 번만 구성한다.
            _transitionPlayers = new IUITransitionPlayer[]
            {
                new UIPanelFadePlayer(_canvasGroup),
                new UIPanelScalePlayer(_rectTransform),
                new UIPanelMovePlayer(_rectTransform)
            };
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 제거될 때 남아 있는 Tween도 같이 정리한다.
        _activeTween?.Kill();
    }
}
