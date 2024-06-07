using System;

using UnityEngine;

public class RuneController : MonoBehaviour, IRecieveDamage
{
    [SerializeField] private int health = 0;

    private int currentHealth = 0;

    private Action onDestroy = null;
    private Action<int, int> onUpdateHealth = null;

    public void Init(Action<int, int> onUpdateHealth, Action onDestroy)
    {
        this.onUpdateHealth = onUpdateHealth;
        this.onDestroy = onDestroy;

        UpdateHealth(health);
    }

    public void RecieveDamage(int damage)
    {
        if (currentHealth <= 0) return;

        UpdateHealth(Mathf.Clamp(currentHealth - damage, 0, health));
        if (currentHealth == 0)
        {
            onDestroy?.Invoke();
        }
    }

    private void UpdateHealth(int health)
    {
        currentHealth = health;
        onUpdateHealth?.Invoke(currentHealth, this.health);
    }
}