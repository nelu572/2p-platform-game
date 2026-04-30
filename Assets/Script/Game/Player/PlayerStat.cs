using UnityEngine;


public class PlayerStat : MonoBehaviour, IDamageable
{

    ///TODO InGameManager에서 부활 함수 작성해야됨
    ///부활 함수는 체력이 0일때 실행하는 함수이며 Life를 1만큼 감소하고
    ///Hp값을 MaxHp값으로 대입 
    ///그후 죽은 캐릭터는 다시 움직일 수 있게 만들어야됨(이건 어떤 스크립트를 만들어야되지)
    //이 프로퍼티는 목숨입니다
    public int Life { get; set; } = 3;
    public int Hp { get; set; } = 100;
    private int _maxHp = 100;
    public int MaxHp { get => _maxHp; }
    
    public int AttackDamage { get; set; } = 20;
    public int SkillDamage { get; set; } = 40;

    //공격 쿨타임
    public float AttackCooltime { get; set; } = 0f;
    public float AttackCooltimeMax { get; set; }
    //스킬 쿨타임
    public float SkillCooltime { get; set; } = 0f;
    public float SkillCooltimeMax { get; set; }

    public int TeamId { get; set; }

    //스텟 초기화 함수(Awake에 단 한번만 실행할 함수)
    public void StatInitialize(int attackDamage, int skillDamage, int maxHp, float attackCooltimeMax, float skillCooltimeMax)
    {
        AttackDamage = attackDamage;
        SkillDamage = skillDamage;
        _maxHp = maxHp;
        //최대 체력만큼 체력을 초기화
        Hp = MaxHp;
        AttackCooltimeMax = attackCooltimeMax;
        SkillCooltimeMax = skillCooltimeMax;
    }

    public void TakeDamage(int attackDamage)
    {
        Hp -= attackDamage;
        if(Hp < 0) Hp = 0;
    }
}
