using System;

using UnityEngine;

public class MeatConsumableItem : ConsumableItem
{
    private int healLives = 0;

    public override void Init(ConsumableData data, Action onTrigger)
    {
        base.Init(data, onTrigger);

        if (data is MeatConsumableData meatData)
        {
            healLives = meatData.HealLives;
        }
    }

    protected override void OnTriggerEvent(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            playerController.IncreaseLives(healLives);
        }
    }
}
