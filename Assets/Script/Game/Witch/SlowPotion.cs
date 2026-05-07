using System.Collections;
using UnityEngine;

public class SlowPotion : Potion
{
    [SerializeField] private float _slowDuration = 5f;
    [SerializeField] private float _slowMagnitude = 0.5f;

    protected override void ApplyEffect(Collider2D hit)
    {
        if (hit.TryGetComponent<PlayerController>(out var target))
        {
            if (target.TeamId != _playerStat.TeamId)
            {
                target.StartCoroutine(ApplySlow(target));
                Debug.Log("속도 감소");
            }
        }
    }

    private IEnumerator ApplySlow(PlayerController target)
    {
        // 이미 슬로우 중이면 중복 적용 안함
        if (target.IsSlowed) yield break;

        target.IsSlowed = true;
        target.MoveSpeed *= _slowMagnitude;

        yield return new WaitForSeconds(_slowDuration);

        target.MoveSpeed /= _slowMagnitude;
        target.IsSlowed = false;
    }
}