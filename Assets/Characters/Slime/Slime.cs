using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] CharacterStatsSO stats;

    [Header("Wandering & Idle")]
    [SerializeField] float minIdleTime = 1f; 
    [SerializeField] float maxIdleTime = 3f;
    [SerializeField] float minWanderTime = 0.5f;
    [SerializeField] float maxWanderTime = 3f;
    [SerializeField] float chanceToIdleAgain = 0.5f;

    private enum SlimeState { Idle, Wandering, Chasing }
    private SlimeState currentState = SlimeState.Idle;

    private float stateTimer = 0f;
    private Vector2 moveDirection = Vector2.zero;

    Rigidbody2D rb;
    Transform target;
    Animator animator;
    SpriteRenderer spriteRenderer;

    private bool isChasing = false;
    private bool canMove = true;
    bool canAttack = true;

    void Awake()
    {
        if (stats == null)
        {
            Debug.LogError($"{name} has no CharacterStatsSO assigned!");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.linearDamping = stats.moveDrag;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        EnterIdle();
    }

    void FixedUpdate()
    {
        if (!GetComponent<DamageableCharacter>().IsAlive) return;

        stateTimer -= Time.fixedDeltaTime;

        rb.linearDamping = moveDirection.sqrMagnitude > 0.01f ? stats.moveDrag : stats.stopDrag;

        if (!canMove)
        {
            moveDirection = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }

        switch (currentState)
        {
            case SlimeState.Chasing:
                if (target != null)
                {
                    moveDirection = ((Vector2)target.position - rb.position).normalized;
                }
                break;

            case SlimeState.Wandering:
                if (stateTimer <= 0f)
                {
                    if (Random.value < chanceToIdleAgain)
                        EnterIdle();
                    else
                        PickWanderDirection();
                }
                break;

            case SlimeState.Idle:
                moveDirection = Vector2.zero;
                if (stateTimer <= 0f)
                {
                    PickWanderDirection();
                }
                break;
        }

        rb.linearVelocity = moveDirection * stats.moveSpeed;

        animator.SetBool("isMoving", moveDirection.sqrMagnitude > 0.01f);

        if (moveDirection.x != 0)
            FlipSprite(moveDirection.x);
    }

    private void EnterIdle()
    {
        currentState = SlimeState.Idle;
        moveDirection = Vector2.zero;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void PickWanderDirection()
    {
        currentState = SlimeState.Wandering;

        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                    Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        stateTimer = Random.Range(minWanderTime, maxWanderTime);
    }

    public void StartChasing(Transform playerTransform)
    {
        target = playerTransform;
        isChasing = true;
        currentState = SlimeState.Chasing;
        moveDirection = ((Vector2)playerTransform.position - rb.position).normalized;
    }

    public void StopChasing()
    {
        target = null;
        isChasing = false;
        EnterIdle();
    }

    public void Freeze()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void Unfreeze()
    {
        canMove = true;
    }

    void FlipSprite(float horizontalDirection)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = horizontalDirection < 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canAttack) return;

        if (collision.gameObject.TryGetComponent<IDamageable>(out var player))
        {
            Vector2 direction = ((Vector2)collision.transform.position - rb.position).normalized;
            Vector2 knockback = direction * stats.knockbackForce;
            player.TakeDamage(stats.damage, knockback);

            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        Freeze();

        yield return new WaitForSeconds(stats.attackCooldown);

        Unfreeze();
        canAttack = true;
    }
}