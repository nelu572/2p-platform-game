public interface IAttackController
{
    float SkillCooltime { get; set; }
    float AttackCooltime { get; set; }
    public void Attack() { }
    public void Skill() { }
}