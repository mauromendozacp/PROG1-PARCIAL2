using UnityEngine;

[CreateAssetMenu(fileName = "PotionConsumable_", menuName = "ScriptableObjects/Consumable/Potion", order = 2)]
public class PotionConsumableData : ConsumableData
{
    [SerializeField][Range(0f, 100f)] private float increaseSpeedPorc = 0f;
    [SerializeField] private float duration = 0f;

    public float IncreaseSpeedPorc { get => increaseSpeedPorc; }
    public float Duration { get => duration; }
}
