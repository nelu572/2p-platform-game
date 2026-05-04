using UnityEngine;
using UnityEngine.InputSystem;

public class ChargeInputHandler : MonoBehaviour
{
    private IChargeable _chargeable;
    private InputAction _attackAction;

    void Awake()
    {
        _chargeable = GetComponent<IChargeable>();

        // PlayerInput에서 Attack 액션 직접 가져오기
        _attackAction = GetComponent<PlayerInput>().actions["Attack"];
    }
    void OnEnable()
    {
        _attackAction.canceled += OnAttack;
    }

    void OnDisable()
    {
        _attackAction.canceled -= OnAttack;
    }

    // 공격키 해재시 발동
    public void OnAttack(InputAction.CallbackContext context)   
    {
        if (context.canceled)
            _chargeable.ReleaseAttack();
    }
}