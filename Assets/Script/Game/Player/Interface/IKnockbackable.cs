using UnityEngine;

public interface IKnockbackable
{
    public bool IsKnockedBack { get; set; }
    public void ApplyKnockback(Vector2 direction, float force);
}
