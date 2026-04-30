using UnityEngine;


public class PlayerStat : MonoBehaviour, IDamageable
{

    ///TODO InGameManager에서 부활 함수 작성해야됨
    ///부활 함수는 체력이 0일때 실행하는 함수이며 Life를 1만큼 감소하고
    ///Hp값을 MaxHp값으로 대입 
    ///그후 죽은 캐릭터는 다시 움직일 수 있게 만들어야됨(이건 어떤 스크립트를 만들어야되지)
    //이 프로퍼티는 목숨입니다
    public int Life { get; set; } = 3;
    public int Hp { get; set; }
    private int _maxHp;
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int AttackDamage { get; set; }
    public int SkillDamage { get; set; }
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }
    public int TeamId { get; set; }

    public void TakeDamage(int attackDamage)
    {
        Hp -= attackDamage;

        if(Hp < 0) Hp = 0;
    }
}
