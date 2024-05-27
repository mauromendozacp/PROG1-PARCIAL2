using System;

using UnityEngine;
using UnityEngine.AI;

public enum FSM_ENEMY
{
    IDLE,
    GO_TO_TARGET,
    ATTACK,
    RECIEVE_DAMAGE,
    DEATH
}

public abstract class EnemyController : MonoBehaviour, IRecieveDamage
{
    [Header("General Settings"), Space]
    [SerializeField] protected int lives = 0;
    [SerializeField] protected int damage = 0;
    [SerializeField] protected float speed = 0f;
    [SerializeField] protected float distanceToAttack = 0f;
    [SerializeField] protected LayerMask attackLayer = default;

    protected NavMeshAgent agent = null;
    protected Transform target = null;

    protected FSM_ENEMY state = default;
    protected int currentLives = 0;

    protected Action<EnemyController> onRelease = null;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    protected void Update()
    {
        UpdateFSM();
    }

    public void Init(Action<EnemyController> onRelease)
    {
        this.onRelease = onRelease;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public abstract void RecieveDamage(int damage);

    protected abstract void UpdateFSM();

    public virtual void OnGet()
    {
        currentLives = lives;
        state = FSM_ENEMY.IDLE;

        gameObject.SetActive(true);
    }

    public virtual void OnRelease()
    {
        gameObject.SetActive(false);
    }
}