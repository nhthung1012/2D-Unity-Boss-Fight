using UnityEngine;

public class ProjectilesAttack : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 8f;

    public void FireAtPlayer(Transform player)
    {
        Vector2 spawnPos = transform.position;

        GameObject projGO = Instantiate(
            projectilePrefab,
            spawnPos,
            Quaternion.identity
        );

        Vector2 dir = ((Vector2)player.position - spawnPos).normalized;

        Projectile projectile = projGO.GetComponent<Projectile>();
        projectile.Launch(dir, projectileSpeed);
    }
}
