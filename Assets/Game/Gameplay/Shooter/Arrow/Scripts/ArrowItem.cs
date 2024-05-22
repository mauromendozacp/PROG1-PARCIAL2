using System;

using UnityEngine;

public class ArrowItem : MonoBehaviour
{
    private Rigidbody rb = null;

    private Action<ArrowItem> onRelease = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        onRelease?.Invoke(this);
    }

    public void Init(Action<ArrowItem> onRelease)
    {
        this.onRelease = onRelease;
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
