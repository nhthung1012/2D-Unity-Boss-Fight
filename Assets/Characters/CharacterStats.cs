using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterStats",
    menuName = "Stats/Character Stats"
)]
public class CharacterStatsSO : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 5;

    [Header("Combat")]
    public int damage = 1;
    public float knockbackForce = 5f;
    public float selfKnockbackForce = 2f;
    public float attackCooldown = 0.6f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveDrag = 5f;
    public float stopDrag = 20f;
}