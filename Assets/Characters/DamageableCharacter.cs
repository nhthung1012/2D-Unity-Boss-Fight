using System.Collections;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Collider2D physicalCollider;

    [Header("Health & Knockback")]
    protected int currentHealth;
    [SerializeField] private float corpseTime = 5f;
    [SerializeField] private float knockbackDuration = 0.5f;
    [SerializeField] private float knockbackDragMultiplier = 1f; // how fast knockback slows down
    [Header("Stats")]
    [SerializeField] protected CharacterStatsSO stats;

    protected bool isAlive = true;
    protected bool targetable = true;

    protected PlayerController playerController;
    protected Slime slime;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicalCollider = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>(); 
        slime = GetComponent<Slime>();

        if (stats == null)
        {
            Debug.LogError($"{name} has no CharacterStatsSO assigned!");
            enabled = false;
            return;
        }

        currentHealth = stats.maxHealth;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    protected virtual void Start()
    {
        if (animator != null)
            animator.SetBool("isAlive", isAlive);
    }

    public int Health
    {
        get => currentHealth;
        set
        {
            int old = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, stats.maxHealth);

            if (currentHealth < old && currentHealth > 0)
            {
                OnDamageTaken();
            }

            if (currentHealth <= 0 && isAlive)
            {
                Die();
            }
        }
    }

    public bool Targetable
    {
        get => targetable;
        set
        {
            targetable = value;
            if (physicalCollider != null)
            {
                physicalCollider.enabled = value;
            }
        }
    }

    public bool IsAlive => isAlive;

    public virtual void TakeDamage(int damage, Vector2 knockback = default)
    {
        if (!targetable || !isAlive) return;

        Health -= damage;

        if (knockback != default && rb != null)
        {
            StartCoroutine(KnockbackRoutine(knockback));
        }
    }

    protected IEnumerator KnockbackRoutine(Vector2 force)
    {
        if (playerController != null) playerController.Freeze();
        if (slime != null) slime.Freeze();

        rb.linearVelocity = Vector2.zero;
        float originalDrag = rb.linearDamping;
        rb.linearDamping = knockbackDragMultiplier; 

        rb.AddForce(force, ForceMode2D.Impulse);
        if (slime) print("Knockback applied: " + force);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearDamping = originalDrag;

        if (playerController != null) playerController.Unfreeze();
        if (slime != null) slime.Unfreeze();
    }

    protected virtual void OnDamageTaken()
    {
        if (animator != null)
            animator.SetTrigger("damage");
    }

    protected virtual void Die()
    {
        isAlive = false;
        Targetable = false;
        print("Character died: " + gameObject.name);

        if (animator != null)
            animator.SetBool("isAlive", false);

        Destroy(gameObject, corpseTime);
    }
}