using UnityEngine;
using UnityEngine.InputSystem;

public class RailgunAttack : AttackCheck
{
    [SerializeField]private float overDriveTime = 0f;// 임시로 스킬 이름을 오버 드라이브로 설정함
    private bool isOverDriveActive = false;
    private Vector2 attackLengthHalfPos;


    public override void OnAttack(InputAction.CallbackContext context)
    {
        if (attackCooltime <= 0f)
        {
            attackLengthHalfPos = new Vector2(50f * direction, 0);
            attackLengthHalfPos += attackPos;

            Collider2D hit = Physics2D.OverlapCapsule(attackLengthHalfPos, new Vector2(100f, 1f), CapsuleDirection2D.Horizontal, 0f, layerMask);
            if (hit != null)// TODO: 오브젝트가 아닌 벽에 먼저 맞으면 공격 무효화 하기
            {
                // 공격이 감지되었을때 처리 코드
            }
                

            if (overDriveTime > 0f)
                attackCooltime = 1f;
            else
                attackCooltime = 5f;
        }
    }

    public override void OnSkill(InputAction.CallbackContext context)
    {
        if (cooltime <= 0f)
        {
            isOverDriveActive = true;
            cooltime = 30f;
        }
    }

    protected override void Update()
    {
        base.Update();

        Debug.Log(direction);
        if (isOverDriveActive)
        {
            overDriveTime = 15f;// 임시로 15초로 설정함
            isOverDriveActive = false;
        }

        if(overDriveTime > 0f)
            overDriveTime -= Time.deltaTime;

        if (cooltime > 0f)
            cooltime -= Time.deltaTime;

        if(attackCooltime > 0f)
            attackCooltime -= Time.deltaTime;
    }
}
