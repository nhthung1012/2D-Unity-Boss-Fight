using UnityEngine;

public interface IDamageable
{
    int Health { get; set; }
    bool Targetable { get; set; }
    void TakeDamage(int damage, Vector2 knockback = default);
}