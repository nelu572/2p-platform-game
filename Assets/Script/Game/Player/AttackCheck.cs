using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class AttackCheck : MonoBehaviour
{
    protected float cooltime = 0f;

    protected float direction;
    protected Vector2 attackOffset = new Vector2(1f, 0f); // 오른쪽 기준 오프셋
    protected Vector2 attackPos;
    public float attackCooltime = 0f;
    protected int layerMask = 1 << 7;//비트 마스킹 용

    protected virtual void Update()
    {
        if(gameObject.transform.localScale.x > 0)
            direction = 1f;
        else if(gameObject.transform.localScale.x < 0)
            direction = -1f;
        attackPos = (Vector2)transform.position + attackOffset * direction;
    }

    public abstract void OnAttack(InputAction.CallbackContext context);
    public abstract void OnSkill(InputAction.CallbackContext context);
}
