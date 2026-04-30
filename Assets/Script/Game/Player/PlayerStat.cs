using UnityEngine;


public class PlayerStat : MonoBehaviour, IDamageable
{
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
    }
}
