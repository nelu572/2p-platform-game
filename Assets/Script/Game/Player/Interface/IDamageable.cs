public interface IDamageable
{
    public int TeamId { get; set; }
    public void TakeDamage(int attackDamage);
}