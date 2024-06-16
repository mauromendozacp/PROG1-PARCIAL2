using System;

using UnityEngine;

public class Arrow : MonoBehaviour
{
    private LayerMask targetLayer = default;
    private BoxCollider boxCollider = null;
    private Rigidbody rb = null;
    private int damage = 0;

    private Action onRelease = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.CheckLayerInMask(targetLayer, collision.gameObject.layer))
        {
            IRecieveDamage recieveDamage = collision.gameObject.GetComponent<IRecieveDamage>();
            recieveDamage?.RecieveDamage(damage);

            onRelease?.Invoke();
        }
    }

    public void Init(LayerMask targetLayer, Action onRelease)
    {
        this.targetLayer = targetLayer;
        this.onRelease = onRelease;

        boxCollider.excludeLayers = ~targetLayer;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void FireArrow(float force, Vector3 direction)
    {
        transform.forward = direction;
        rb.AddForce(force * direction, ForceMode.Impulse);
    }

    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);

        rb.velocity = Vector3.zero;
    }
}
