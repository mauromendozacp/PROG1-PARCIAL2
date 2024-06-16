using UnityEngine;
using UnityEngine.Pool;

public class BorrowController : MonoBehaviour
{
    [SerializeField] private Arrow arrowPrefab = null;
    [SerializeField] private Transform spawnPosition = null;
    [SerializeField] private Transform poolHolder = null;
    [SerializeField] private LayerMask targetLayer = default;
    [SerializeField] private int damage = 0;

    private ObjectPool<Arrow> arrowPool = null;

    private void Start()
    {
        arrowPool = new ObjectPool<Arrow>(CreateArrow, GetArrow, ReleaseArrow, DestroyArrow);
    }

    public void FireArrow(float force, Vector3 direction)
    {
        Arrow arrow = arrowPool.Get();
        arrow.SetDamage(damage);
        arrow.FireArrow(force, direction);
    }

    private Arrow CreateArrow()
    {
        Arrow arrowItem = Instantiate(arrowPrefab, poolHolder);
        arrowItem.Init(targetLayer, () => arrowPool.Release(arrowItem));

        return arrowItem;
    }

    private void GetArrow(Arrow arrow)
    {
        arrow.transform.position = spawnPosition.position;
        arrow.OnGet();
    }

    private void ReleaseArrow(Arrow arrow)
    {
        arrow.OnRelease();
    }

    private void DestroyArrow(Arrow arrow)
    {
        Destroy(arrow.gameObject);
    }
}
