using System.Collections;
using UnityEngine;

public class SlowPotion : Potion
{
    protected override Color AreaColor => new Color(0.2f, 0.45f, 1f, 1f);

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

        float multiplier = 1f - _slowMagnitude;
        target.MoveSpeed *= multiplier;

        yield return new WaitForSeconds(_slowDuration);

        target.MoveSpeed /= multiplier;
        target.IsSlowed = false;
    }
}
