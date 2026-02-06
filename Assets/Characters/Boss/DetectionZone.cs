using UnityEngine;

public class BossDetectionZone : MonoBehaviour
{
    BossController boss;

    void Awake()
    {
        boss = GetComponentInParent<BossController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boss.ActivateBoss(other.transform);
        }
    }
}
