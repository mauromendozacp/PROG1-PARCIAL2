using System;

using UnityEngine;

public class RuneController : MonoBehaviour, IRecieveDamage
{
    [SerializeField] private int health = 0;

    private Action onDestroy = null;

    public void Init(Action onDestroy)
    {
        this.onDestroy = onDestroy;
    }

    public void RecieveDamage(int damage)
    {
        if (health <= 0) return;

        health = Mathf.Clamp(health - damage, 0, health);
        if (health == 0)
        {
            health = 0;

            onDestroy?.Invoke();
        }
    }
}