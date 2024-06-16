using System;

using UnityEngine;

public abstract class ConsumableItem : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer = default;

    private Action onTrigger = null;

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.CheckLayerInMask(playerLayer, other.gameObject.layer))
        {
            OnTriggerEvent(other);
            onTrigger?.Invoke();
        }
    }

    public virtual void Init(ConsumableData data, Action onTrigger)
    {
        this.onTrigger = onTrigger;
    }

    protected abstract void OnTriggerEvent(Collider other);

    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }
}
