using System;

using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private LayerMask impactLayer = default;
    [SerializeField] private float attackRadius = 0f;

    [SerializeField] private GameObject impactRockParticleGO = null;
    [SerializeField] private float particleDuration = 0f;

    private Rigidbody rigid = null;
    private SphereCollider sphereCollider = null;
    private LayerMask attackLayer = default;
    private int damage = 0;

    private Action onDespawn = null;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        TogglePhysics(false);
        Toggle(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.CheckLayerInMask(impactLayer, collision.gameObject.layer))
        {
            ApplyAttack();
            TogglePhysics(false);
            Toggle(false);
            CreateImpactRockParticle();

            onDespawn?.Invoke();
        }
    }

    public void Init(LayerMask attackLayer, int damage, Action onDespawn)
    {
        this.attackLayer = attackLayer;
        this.damage = damage;
        this.onDespawn = onDespawn;
    }

    public void ApplyForce(Vector3 force)
    {
        TogglePhysics(true);
        rigid.AddForce(force, ForceMode.Impulse);
    }

    public void Toggle(bool status)
    {
        gameObject.SetActive(status);
    }

    private void TogglePhysics(bool status)
    {
        rigid.isKinematic = !status;
        sphereCollider.enabled = status;
    }

    private void ApplyAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius, attackLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null && colliders[i].TryGetComponent(out IRecieveDamage recieveDamage))
            {
                recieveDamage.RecieveDamage(damage);
            }
        }
    }

    private void CreateImpactRockParticle()
    {
        GameObject particleGO = Instantiate(impactRockParticleGO);
        particleGO.transform.position = transform.position;

        Destroy(particleGO, particleDuration);
    }
}
