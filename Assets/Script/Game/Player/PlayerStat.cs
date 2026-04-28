using UnityEngine;


public class PlayerStat : MonoBehaviour, IDamageable
{
    public float Hp { get; set; }
    private float _maxHp;
    public float MaxHp => _maxHp;
    public float Defence { get; set; } = 30f;
    public float AttackDamage { get; set; } = 50f;
    public float SkillDamage { get; set; }
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }
    public int TeamId { get; set; }

    public void TakeDamage(int attackDamage)
    {
        Hp -= (Defence - attackDamage);
    }
}
