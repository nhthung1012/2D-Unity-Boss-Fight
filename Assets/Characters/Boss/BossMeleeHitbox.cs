using UnityEngine;

public class BossMeleeHitbox : MonoBehaviour
{
    [SerializeField] int damage = 2;
    [SerializeField] float knockbackForce = 6f;

    Collider2D hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
    }

    public void EnableHitbox()
    {
        hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            target.TakeDamage(damage, dir * knockbackForce);
        }
    }
}
