using UnityEngine;

[CreateAssetMenu(fileName = "MeatConsumable_", menuName = "ScriptableObjects/Consumable/Meat", order = 1)]
public class MeatConsumableData : ConsumableData
{
    [SerializeField] private int healLives = 0;

    public int HealLives { get => healLives; }
}
