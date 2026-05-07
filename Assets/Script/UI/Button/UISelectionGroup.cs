using UnityEngine;

/// <summary>
/// 현재 화면의 기본 선택 버튼을 관리한다.
/// UIInput은 입력만 담당하고, 어떤 버튼을 먼저 선택할지는 이 그룹이 넘겨준다.
/// </summary>
public class UISelectionGroup : MonoBehaviour
{
    [SerializeField] private UIInput _uiInput;
    [SerializeField] private UIButton _defaultButton;

    private void OnEnable()
    {
        _uiInput ??= FindFirstObjectByType<UIInput>();

        if (_uiInput == null)
        {
            return;
        }

        _uiInput.SetDefaultButton(_defaultButton);
        _uiInput.SelectDefaultButton();
    }
}
