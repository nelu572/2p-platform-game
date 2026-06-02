using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class BaseAttackController : MonoBehaviour, IAttackController
{
    protected PlayerController PlayerController { get; private set; }
    protected PlayerStat PlayerStat { get; private set; }
    protected BoxCollider2D BodyCollider { get; private set; }
    protected List<Collider2D> HitBuffer { get; } = new List<Collider2D>(15);
    protected ContactFilter2D ContactFilter { get; private set; }

    protected virtual void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerStat = GetComponent<PlayerStat>();
        BodyCollider = GetComponent<BoxCollider2D>();

        PlayerController.OnAttackHandler = Attack;
        PlayerController.OnSkillHandler = Skill;
    }

    public abstract void Attack();
    public abstract void Skill();

    protected bool IsAttackOnCooldown()
    {
        return PlayerStat.AttackCooltime > 0f;
    }

    protected bool IsSkillOnCooldown()
    {
        return PlayerStat.SkillCooltime > 0f;
    }

    protected void StartAttackCooldown()
    {
        PlayerStat.AttackCooltime = PlayerStat.AttackCooltimeMax;
    }

    protected void StartSkillCooldown()
    {
        PlayerStat.SkillCooltime = PlayerStat.SkillCooltimeMax;
    }

    protected Vector2 GetFacingDirection()
    {
        return transform.localScale.x > 0f ? Vector2.right : Vector2.left;
    }

    protected bool IsFacingRight()
    {
        return transform.localScale.x > 0f;
    }

    protected void SetupContactFilter(LayerMask layerMask)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layerMask);
        contactFilter.useTriggers = true;
        ContactFilter = contactFilter;
    }

    protected void OverlapBox(Vector2 origin, Vector2 size)
    {
        HitBuffer.Clear();
        Physics2D.OverlapBox(origin, size, 0f, ContactFilter, HitBuffer);
    }

    protected Vector2 GetHorizontalBoxOrigin(BoxCollider2D offsetCollider, Vector2 attackSize)
    {
        Vector2 facingDir = GetFacingDirection();
        float offsetX = (offsetCollider.offset.x + offsetCollider.size.x / 2f + attackSize.x / 2f) * facingDir.x;
        return (Vector2)transform.position + new Vector2(offsetX, 0f);
    }

    protected bool TryGetEnemyStat(Collider2D target, out PlayerStat enemyStat)
    {
        enemyStat = null;

        if (target.gameObject == gameObject)
            return false;

        if (!target.TryGetComponent(out enemyStat))
            return false;

        return enemyStat.TeamId != PlayerStat.TeamId;
    }
}
