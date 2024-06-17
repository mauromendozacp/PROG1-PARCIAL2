using System;

using UnityEngine;

public abstract class ConsumableItem : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer = default;
    [SerializeField] private AudioEvent consumeEvent = null;

    private Action onTrigger = null;

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.CheckLayerInMask(playerLayer, other.gameObject.layer))
        {
            GameManager.Instance.AudioManager.PlayAudio(consumeEvent, transform.position);

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
