using UnityEngine;

public class TreeSorting : MonoBehaviour
{
    private SpriteRenderer treeRenderer;
    public Transform playerTransform;
    public Transform slimeTransform;

    void Start()
    {
        treeRenderer = GetComponent<SpriteRenderer>();
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned!");
        }
        if (slimeTransform == null)
        {
            Debug.LogError("Slime Transform not assigned!");
        }
    }

    void Update()
    {
        if (playerTransform.position.y < transform.position.y)
        {
            treeRenderer.sortingOrder = 0;
        }
        else
        {
            treeRenderer.sortingOrder = 2;
        }

        if (slimeTransform.position.y < transform.position.y)
        {
            treeRenderer.sortingOrder = 0;
        }
        else
        {
            treeRenderer.sortingOrder = 2;
        }
    }
}
