using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class BaseAttackController : MonoBehaviour, IAttackController
{
    protected PlayerController PlayerController { get; private set; }
    protected PlayerStat PlayerStat { get; private set; }
    protected BoxCollider2D BodyCollider { get; private set; }
    protected List<Collider2D> HitBuffer { get; } = new List<Collider2D>(15);
    protected ContactFilter2D ContactFilter { get; private set; }
    private readonly Dictionary<Type, VisibleAttack> _visibleAttacks = new Dictionary<Type, VisibleAttack>();

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

    protected T GetChildVisibleAttack<T>(string childName) where T : VisibleAttack
    {
        Type type = typeof(T);
        if (_visibleAttacks.TryGetValue(type, out VisibleAttack cachedAttack))
            return (T)cachedAttack;

        T visibleAttack = null;
        T[] visibleAttackComponents = GetComponentsInChildren<T>(true);
        for (int i = 0; i < visibleAttackComponents.Length; i++)
        {
            if (visibleAttackComponents[i].transform == transform)
                continue;

            visibleAttack = visibleAttackComponents[i];
            break;
        }

        if (visibleAttack == null)
        {
            Debug.LogWarning($"{childName} 자식 오브젝트가 없어 공격 범위 표시를 건너뜁니다.", this);
            _visibleAttacks[type] = null;
            return null;
        }

        visibleAttack.Hide();
        _visibleAttacks[type] = visibleAttack;
        return visibleAttack;
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
        if(offsetCollider == null)
            return (Vector2) transform.position;

        Vector2 facingDir = GetFacingDirection();
        float offsetX = (offsetCollider.offset.x + offsetCollider.size.x / 2f + attackSize.x / 2f) * facingDir.x;
        return (Vector2)transform.position + new Vector2(offsetX, 0f);
    }

    protected bool TryGetEnemyStat(Collider2D target, out PlayerStat enemyStat)
    {
        enemyStat = null;

        if (target == null || PlayerStat == null || target.gameObject == gameObject)
            return false;

        if (!target.TryGetComponent(out enemyStat))
            return false;

        return enemyStat.TeamId != PlayerStat.TeamId;
    }
}
