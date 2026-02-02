using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterStatsSO stats;

    [Header("Attack")]
    public PlayerAttack playerAttack;

    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.linearDamping = stats.stopDrag;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (stats == null)
        {
            Debug.LogError($"{name} has no CharacterStatsSO assigned!");
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        if (movementInput != Vector2.zero) {
            rb.linearDamping = stats.moveDrag;
        }
        else {
            rb.linearDamping = stats.stopDrag;
        }
        if (canMove)
        {
            rb.linearDamping = movementInput == Vector2.zero ? stats.stopDrag : stats.moveDrag;
            rb.linearVelocity = movementInput * stats.moveSpeed;

            animator.SetBool("isMoving", movementInput != Vector2.zero);

            if (movementInput.x != 0)
                FlipSprite();
        }
        else
        {
            // Set Linear Damping to move drag to make player knockback feel better
            rb.linearDamping = stats.moveDrag;
            animator.SetBool("isMoving", false);
        }
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(movementInput.x);
        transform.localScale = scale;

        playerAttack.facingDirection =
            scale.x > 0 ? PlayerAttack.FacingDirection.Right : PlayerAttack.FacingDirection.Left;
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnAttack()
    {
        animator.SetTrigger("attack");
    }

    public void Attack() { Freeze(); playerAttack.Attack(); }
    public void EndAttack() { Unfreeze(); playerAttack.EndAttack(); }

    public void Freeze() 
    { 
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }
    public void Unfreeze() 
    { 
        canMove = true; 
    }
}