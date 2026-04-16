public interface IAttackController
{
    public float SkillCooltime { get; set; }
    public float AttackCooltime { get; set; }
    public void Attack() { }
    public void Skill() { }
}