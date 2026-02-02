using UnityEngine;

public class SlimeDetectionZone : MonoBehaviour
{
    private Slime slime;
    
    void Awake()
    {
        slime = GetComponentInParent<Slime>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        slime.StartChasing(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        slime.StopChasing();
    }
}
