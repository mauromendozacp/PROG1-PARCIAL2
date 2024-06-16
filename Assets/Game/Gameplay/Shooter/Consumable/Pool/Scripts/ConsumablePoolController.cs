using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

public class ConsumablePoolController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer = default;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private ConsumableData[] consumables = null;

    private Dictionary<string, ObjectPool<ConsumableItem>> consumablePoolDict = null;
    private List<ConsumableItem> consumableList = null;

    public void Init()
    {
        consumablePoolDict = new Dictionary<string, ObjectPool<ConsumableItem>>();
        consumableList = new List<ConsumableItem>();

        for (int i = 0; i < consumables.Length; i++) 
        {
            ConsumableData data = consumables[i];

            GameObject consumableHolder = new GameObject(data.Id.ToLower() + "_holder");
            consumableHolder.transform.SetParent(transform);

            consumablePoolDict.Add(data.Id, new ObjectPool<ConsumableItem>(
                createFunc: () =>
                {
                    ConsumableItem item = Instantiate(data.Prefab, consumableHolder.transform);
                    item.Init(data, onTrigger: () => consumablePoolDict[data.Id].Release(item));

                    return item;
                },
                GetConsumable, ReleaseConsumable, DestroyConsumable));
        }
    }

    public void TryDropConsumable(Vector3 position)
    {
        ConsumableItem item = GetChanceConsumable();
        if (item != null)
        {
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                item.transform.position = hit.point + spawnOffset;
            }
        }
    }

    private ConsumableItem GetChanceConsumable()
    {
        if (consumables == null || consumables.Length == 0) return null;

        int randomIndex = Random.Range(0, consumables.Length);
        ConsumableData consumable = consumables[randomIndex];

        int randomChance = Random.Range(0, 100);
        if (randomChance < consumable.DropChance)
        {
            return consumablePoolDict[consumable.Id].Get();
        }

        return null;
    }

    private void GetConsumable(ConsumableItem consumable)
    {
        consumable.OnGet();
        consumableList.Add(consumable);
    }

    private void ReleaseConsumable(ConsumableItem consumable)
    {
        consumable.OnRelease();
        consumableList.Remove(consumable);
    }

    private void DestroyConsumable(ConsumableItem consumable)
    {
        Destroy(consumable.gameObject);
    }
}
