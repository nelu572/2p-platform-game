using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    protected Animator PlayerAnima;
    void Start()
    {
        PlayerAnima = GetComponent<Animator>();
    }
    public virtual void Attack()
    {
        Debug.Log("공격");
    }
    public virtual void Skill()
    {
        Debug.Log("스킬");
    }
}
