using UnityEngine;

public class BaseballPlayerAction : PlayerAction
{
    public override void Attack()
    {
        Debug.Log("배트 공격");
        PlayerAnima.SetTrigger("Attack");
    }
    public override void Skill()
    {
        Debug.Log("배트 후리기");
    }
}
