using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] CharacterStatsSO stats;
    public enum FacingDirection
    {
        Right,
        Left
    }
    public FacingDirection facingDirection;
    public Collider2D attackCollider;
    Vector2 rightAttackOffset;

    PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        attackCollider.enabled = false;
        rightAttackOffset = transform.position;

        if (stats == null)
        {
            Debug.LogError($"{name} has no CharacterStatsSO assigned!");
            return;
        }
    }

    public void Attack()
    {
        attackCollider.enabled = true;
    }

    public void EndAttack()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var damageable))
        {
            // Enemy knockback from player center -> hit point
            Vector2 playerCenter = transform.root.position;
            Vector2 direction = ((Vector2)collision.transform.position - playerCenter).normalized;
            Vector2 knockback = direction * stats.knockbackForce;

            damageable.TakeDamage(stats.damage, knockback);

            // Self-knockback in opposite direction
            if (transform.root.TryGetComponent<IDamageable>(out var selfDamageable))
            {
                Vector2 selfKnockback = -direction * stats.selfKnockbackForce;
                selfDamageable.TakeDamage(0, selfKnockback);
            }
        }
    }
}