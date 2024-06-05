using System;

using UnityEngine;
using UnityEngine.AI;

public enum FSM_ENEMY
{
    IDLE,
    GO_TO_TARGET,
    ATTACK,
    RECIEVE_DAMAGE,
    DEATH,
    WIN
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
    protected Transform mainTarget = null;
    protected Transform secondaryTarget = null;

    protected FSM_ENEMY state = default;
    protected int currentLives = 0;

    protected Action<EnemyController> onRelease = null;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        ToggleAgent(false);
    }

    protected void Update()
    {
        UpdateFSM();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != FSM_ENEMY.WIN && Utils.CheckLayerInMask(attackLayer, other.gameObject.layer))
        {
            secondaryTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (secondaryTarget == other.transform)
        {
            secondaryTarget = null;
        }
    }

    public void Init(Action<EnemyController> onRelease)
    {
        this.onRelease = onRelease;
    }

    public void SetMainTarget(Transform mainTarget)
    {
        this.mainTarget = mainTarget;
    }

    public void ToggleAgent(bool status)
    {
        agent.enabled = status;
    }

    public virtual void SetWinState()
    {
        state = FSM_ENEMY.WIN;

        mainTarget = null;
        secondaryTarget = null;
    }

    protected Transform GetFocusTarget()
    {
        return secondaryTarget != null ? secondaryTarget : mainTarget;
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
        ToggleAgent(false);
    }
}