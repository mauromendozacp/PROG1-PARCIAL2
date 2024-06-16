using UnityEngine;

public abstract class ConsumableData : ScriptableObject
{
    [SerializeField] protected string id = string.Empty;
    [SerializeField] protected ConsumableItem prefab = null;
    [SerializeField][Range(1, 100)] protected int dropChance = 0;

    public string Id { get => id; }
    public ConsumableItem Prefab { get => prefab; }
    public int DropChance { get => dropChance; }
}
