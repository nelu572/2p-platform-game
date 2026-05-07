using System.Collections;
using UnityEngine;

public class PoisonPotion : Potion
{
    [SerializeField] private float _poisonDuration = 5f; // 지속 시간

    protected override void ApplyEffect(Collider2D hit)
    {
        if (hit.TryGetComponent<PlayerStat>(out var targetStat))
        {
            if (targetStat.TeamId != _playerStat.TeamId)
            {
                hit.TryGetComponent<MonoBehaviour>(out var mono);
                mono.StartCoroutine(ApplyPoison(targetStat));
            }
        }
    }

    private IEnumerator ApplyPoison(PlayerStat targetStat)
    {
        if (targetStat.IsPoisoned) yield break; // 중복 방지

        targetStat.IsPoisoned = true;

        float elapsed = 0f;
        while (elapsed < _poisonDuration)
        {
            targetStat.TakeDamage(_playerStat.AttackDamage / 2); // 초당 공격력 1/2 데미지
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        targetStat.IsPoisoned = false;
    }
}