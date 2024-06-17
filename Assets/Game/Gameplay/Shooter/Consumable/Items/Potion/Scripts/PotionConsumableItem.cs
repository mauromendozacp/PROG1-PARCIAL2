using System;

using UnityEngine;

public class PotionConsumableItem : ConsumableItem
{
    private float increaseSpeedPorc = 0f;
    private float duration = 0f;

    public override void Init(ConsumableData data, Action onTrigger)
    {
        base.Init(data, onTrigger);

        if (data is PotionConsumableData potionData)
        {
            increaseSpeedPorc = potionData.IncreaseSpeedPorc;
            duration = potionData.Duration;
        }
    }

    protected override void OnTriggerEvent(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            playerController.IncreaseAttackSpeed(increaseSpeedPorc, duration);
        }
    }
}
