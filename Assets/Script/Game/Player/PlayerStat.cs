using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    ///TODO InGameManager에서 부활 함수 작성해야됨
    ///부활 함수는 체력이 0일때 실행하는 함수이며 Life를 1만큼 감소하고
    ///Hp값을 MaxHp값으로 대입 
    ///그후 죽은 캐릭터는 다시 움직일 수 있게 만들어야됨

    [SerializeField] private CharacterStatData _statData;

    public int Life { get => _statData.life; set => _statData.life = value; }
    public int Hp { get; set; }
    public int MaxHp => _statData._maxHp;
    public int AttackDamage => _statData._attackDamage;
    public int SkillDamage => _statData._skillDamage;
    public float AttackCooltimeMax => _statData._attackCooltimeMax;
    public float SkillCooltimeMax => _statData._skillCooltimeMax;

    public float AttackCooltime { get; set; } = 0f;
    public float SkillCooltime { get; set; } = 0f;
    public float MoveSpeed { get => _statData._moveSpeed; set => _statData._moveSpeed = value; }

    // 포션 디버프들
    public bool IsSlowed { get; set; }
    public bool IsPoisoned { get; set; }

    public int TeamId { get; set; }

    protected void OnAwake()
    {
        Hp = _statData._maxHp;
    }

    // 쿨타임 감소는 스탯 자신이 관리
    void Update()
    {
        if (AttackCooltime > 0) AttackCooltime -= Time.deltaTime;
        if (SkillCooltime > 0) SkillCooltime -= Time.deltaTime;
    }

    public void TakeDamage(int attackDamage)
    {
        Hp -= attackDamage;
        if (Hp < 0) Hp = 0;
        Debug.Log($"{attackDamage} 만큼 피해를 입었습니다, 남은HP: {Hp}");
    }
}