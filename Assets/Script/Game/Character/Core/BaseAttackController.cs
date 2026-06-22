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
    private readonly HashSet<VisibleAttack> _ownedVisibleAttacks = new HashSet<VisibleAttack>();

    protected virtual void Awake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerStat = GetComponent<PlayerStat>();
        BodyCollider = GetComponent<BoxCollider2D>();

        PlayerController.OnAttackHandler = Attack;
        PlayerController.OnSkillHandler = Skill;
    }

    protected virtual void OnDestroy()
    {
        foreach (VisibleAttack visibleAttack in _ownedVisibleAttacks)
        {
            if (visibleAttack == null)
                continue;

            DestroyVisibleAttack(visibleAttack);
        }

        _ownedVisibleAttacks.Clear();
        _visibleAttacks.Clear();
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

    protected T GetVisibleAttack<T>(string childName) where T : VisibleAttack
    {
        Type type = typeof(T);
        if (_visibleAttacks.TryGetValue(type, out VisibleAttack cachedAttack) && cachedAttack != null)
            return (T)cachedAttack;

        GameObject indicator = new GameObject(childName);
        indicator.transform.position = transform.position;
        indicator.transform.rotation = Quaternion.identity;
        indicator.transform.localScale = Vector3.one;
        T visibleAttack = indicator.AddComponent<T>();
        visibleAttack.Hide();
        _ownedVisibleAttacks.Add(visibleAttack);
        _visibleAttacks[type] = visibleAttack;
        return visibleAttack;
    }

    private void DestroyVisibleAttack(VisibleAttack visibleAttack)
    {
        if (Application.isPlaying)
        {
            Destroy(visibleAttack.gameObject);
            return;
        }

#if UNITY_EDITOR
        if (!UnityEditor.EditorUtility.IsPersistent(visibleAttack.gameObject))
            Destroy(visibleAttack.gameObject);
#endif
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
