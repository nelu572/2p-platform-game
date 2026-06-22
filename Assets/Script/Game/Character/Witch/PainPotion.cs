using UnityEngine;

public class PainPotion : Potion
{
    protected override Color AreaColor => new Color(1f, 0.1f, 0.1f, 1f);

    protected override void ApplyEffect(Collider2D hit)
    {
        if (hit.TryGetComponent<PlayerStat>(out var target))
        {
            if (target.TeamId != _playerStat.TeamId)
            {
                target.TakeDamage(_playerStat.AttackDamage);
                Debug.Log($"PainPotion applied to {target.gameObject.name} for {_playerStat.AttackDamage} damage.");
            }
        }
    }
}
