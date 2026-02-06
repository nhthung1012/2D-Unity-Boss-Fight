using UnityEngine;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Attacking,
        Stunned,
        Dead
    }

    public enum BossPhase
    {
        Phase1,
        Phase2,
        Phase3
    }

    enum AttackType
    {
        Melee,
        Ranged,
        Laser,
        Summon
    }

    [Header("Stats")]
    [SerializeField] CharacterStatsSO stats;

    [Header("Combat")]
    [SerializeField] float decisionCooldown = 0.5f;
    [SerializeField] float meleeRange = 3f;
    [SerializeField] HealthBar healthBar;

    Animator animator;
    SpriteRenderer spriteRenderer;
    DamageableCharacter damageable;
    Transform player;
    BossMeleeHitbox meleeHitbox;
    ProjectilesAttack projectileAttack;
    SummonAttack summonAttack;


    BossState state = BossState.Idle;
    BossPhase phase = BossPhase.Phase1;

    float decisionTimer;
    bool combatActive;

    void Awake()
    {
        animator = GetComponent<Animator>();
        damageable = GetComponent<DamageableCharacter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        meleeHitbox = GetComponentInChildren<BossMeleeHitbox>();
        projectileAttack = GetComponent<ProjectilesAttack>();
        summonAttack = GetComponent<SummonAttack>();

        damageable.OnDeath += OnDeath;
    }

    void Update()
    {
        if (!combatActive || state == BossState.Dead) return;

        FacePlayer();
        UpdatePhase();

        if (state != BossState.Idle) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            ChooseAttack();
        }
    }

    void UpdatePhase()
    {
        if (state == BossState.Attacking) return;

        float hpPercent = (float)damageable.Health / damageable.MaxHealth;

        if (hpPercent <= 0.5f)
            phase = BossPhase.Phase2;
        // else if (hpPercent <= 0.6f)
        //     phase = BossPhase.Phase2;
        else
            phase = BossPhase.Phase1;
    }

    void SetState(BossState newState)
    {
        state = newState;
    }

    void ChooseAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= meleeRange)
        {
            StartAttack("melee");
            return;
        }

        AttackType attack = ChooseRangedAttackByPhase();

        switch (attack)
        {
            case AttackType.Ranged:
                StartAttack("fireProjectile");
                break;

            case AttackType.Laser:
                StartAttack("laser");
                break;

            case AttackType.Summon:
                StartAttack("summon");
                break;
        }
    }

    AttackType ChooseRangedAttackByPhase()
    {
        int roll = Random.Range(0, 100);

        switch (phase)
        {
            case BossPhase.Phase1:
                // Only ranged
                return AttackType.Ranged;

            case BossPhase.Phase2:
                if (roll < 60) return AttackType.Ranged;
                else return AttackType.Summon;

                // case BossPhase.Phase3:
                //     // Ranged / Laser / Summon
                //     if (roll < 30) return AttackType.Ranged;
                //     else if (roll < 60) return AttackType.Laser;
                //     else return AttackType.Summon;
        }

        return AttackType.Ranged;
    }

    void StartAttack(string trigger)
    {
        SetState(BossState.Attacking);
        animator.SetTrigger(trigger);
    }

    public void FireProjectile()
    {
        projectileAttack.FireAtPlayer(player);
    }

    public void SummonSlimes()
    {
        summonAttack.Summon();
    }

    public void EnableMeleeHitbox()
    {
        meleeHitbox.EnableHitbox();
    }

    public void DisableMeleeHitbox()
    {
        meleeHitbox.DisableHitbox();
    }

    public void EndAttack()
    {
        ClearAttackTriggers();
        SetState(BossState.Idle);
        decisionTimer = decisionCooldown;
    }

    public void EnterSummonState()
    {
        damageable.Targetable = false;
        SetState(BossState.Attacking);
    }

    public void ExitSummonState()
    {
        damageable.Targetable = true;
    }

    public void ExitDamage()
    {
        if (state == BossState.Dead) return;

        ClearAttackTriggers();
        SetState(BossState.Idle);
        decisionTimer = decisionCooldown;
    }

    void ClearAttackTriggers()
    {
        animator.ResetTrigger("melee");
        animator.ResetTrigger("fireProjectile");
        animator.ResetTrigger("laser");
        animator.ResetTrigger("summon");
    }

    public void ActivateBoss(Transform playerTransform)
    {
        combatActive = true;
        player = playerTransform;
        SetState(BossState.Idle);
        decisionTimer = decisionCooldown;
        healthBar.Show();
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (player.position.x > transform.position.x ? 1 : -1);
        transform.localScale = scale;
    }

    void OnDeath()
    {
        SetState(BossState.Dead);
        animator.SetBool("isAlive", false);
        healthBar.Hide();
    }
}
