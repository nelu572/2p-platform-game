using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    ///TODO InGameManager에서 부활 함수 작성해야됨
    ///부활 함수는 체력이 0일때 실행하는 함수이며 Life를 1만큼 감소하고
    ///Hp값을 MaxHp값으로 대입 
    ///그후 죽은 캐릭터는 다시 움직일 수 있게 만들어야됨

    [Header("스탯 설정")]
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private int _skillDamage = 40;
    [SerializeField] private float _attackCooltimeMax = 0.8f;
    [SerializeField] private float _skillCooltimeMax = 10f;

    public int Life { get; set; } = 3;
    public int Hp { get; set; }
    public int MaxHp => _maxHp;
    public int AttackDamage => _attackDamage;
    public int SkillDamage => _skillDamage;
    public float AttackCooltimeMax => _attackCooltimeMax;
    public float SkillCooltimeMax => _skillCooltimeMax;

    public float AttackCooltime { get; set; } = 0f;
    public float SkillCooltime { get; set; } = 0f;
    public int TeamId { get; set; }

    void Awake()
    {
        Hp = _maxHp;
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
    }
}