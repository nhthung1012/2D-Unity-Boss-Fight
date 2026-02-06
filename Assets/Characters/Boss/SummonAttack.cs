using UnityEngine;

public class SummonAttack : MonoBehaviour
{
    [SerializeField] GameObject slimePrefab;
    [SerializeField] int summonCount = 3;

    public void Summon()
    {
        for (int i = 0; i < summonCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 5f;
            Instantiate(slimePrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }
    }
}
