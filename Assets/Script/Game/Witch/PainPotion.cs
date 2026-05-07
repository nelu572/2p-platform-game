using UnityEngine;

public class PainPotion : Potion
{
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