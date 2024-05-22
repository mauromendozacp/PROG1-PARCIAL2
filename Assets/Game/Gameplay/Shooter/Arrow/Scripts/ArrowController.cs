using UnityEngine;
using UnityEngine.Pool;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private ArrowItem arrowPrefab = null;
    [SerializeField] private Transform spawnPosition = null;
    [SerializeField] private Transform poolHolder = null;

    private ObjectPool<ArrowItem> arrowPool = null;

    private void Start()
    {
        arrowPool = new ObjectPool<ArrowItem>(CreateArrow, GetArrow, ReleaseArrow, DestroyArrow);
    }

    public void FireArrow(float force, Vector3 direction)
    {
        ArrowItem arrow = arrowPool.Get();
        arrow.transform.position = spawnPosition.position;

        arrow.FireArrow(force, direction);
    }

    private ArrowItem CreateArrow()
    {
        ArrowItem arrowItem = Instantiate(arrowPrefab, poolHolder);
        arrowItem.Init((arrow) => arrowPool.Release(arrow));

        return arrowItem;
    }

    private void GetArrow(ArrowItem arrow)
    {
        arrow.OnGet();
    }

    private void ReleaseArrow(ArrowItem arrow)
    {
        arrow.OnRelease();
    }

    private void DestroyArrow(ArrowItem arrow)
    {
        Destroy(arrow.gameObject);
    }
}
