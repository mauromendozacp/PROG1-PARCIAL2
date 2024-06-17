using System;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum FSM_ENEMY
{
    IDLE,
    GO_TO_TARGET,
    ATTACK,
    HURT,
    DEATH,
    WIN
}

public abstract class EnemyController : MonoBehaviour, IRecieveDamage
{
    [Header("General Settings"), Space]
    [SerializeField] protected int damage = 0;
    [SerializeField] protected float speed = 0f;
    [SerializeField] protected float distanceToAttack = 0f;
    [SerializeField] protected float rotationSpeed = 0f;
    [SerializeField] protected LayerMask attackLayer = default;

    [Header("Life Settings"), Space]
    [SerializeField] protected int lives = 0;
    [SerializeField] protected Slider healthBarSlider = null;

    [Header("Sounds Settings")]
    [SerializeField] protected AudioEvent attackEvent = null;
    [SerializeField] protected AudioEvent hurtEvent = null;
    [SerializeField] protected AudioEvent deathEvent = null;

    protected Camera mainCamera = null;
    protected NavMeshAgent agent = null;
    protected Transform mainTarget = null;
    protected Transform secondaryTarget = null;

    protected FSM_ENEMY state = default;
    protected int currentLives = 0;

    protected Action onKill = null;
    protected Action onRelease = null;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        ToggleAgent(false);
    }

    protected void Update()
    {
        UpdateFSM();
        LookBarToCamera();
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

    public void Init(Camera mainCamera, Action onKill, Action onRelease)
    {
        this.mainCamera = mainCamera;
        this.onKill = onKill;
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

    protected virtual void Attack()
    {
        state = FSM_ENEMY.ATTACK;

        GameManager.Instance.AudioManager.PlayAudio(attackEvent, transform.position);
    }

    protected virtual void Hurt()
    {
        state = FSM_ENEMY.HURT;

        GameManager.Instance.AudioManager.PlayAudio(hurtEvent, transform.position);
    }

    protected virtual void Death()
    {
        onKill?.Invoke();
        state = FSM_ENEMY.DEATH;

        GameManager.Instance.AudioManager.PlayAudio(deathEvent, transform.position);
    }

    protected Transform GetFocusTarget()
    {
        return secondaryTarget != null ? secondaryTarget : mainTarget;
    }

    protected void UpdateLife(int lives)
    {
        currentLives = lives;
        healthBarSlider.value = (float)currentLives / this.lives;
    }

    private void LookBarToCamera()
    {
        healthBarSlider.transform.LookAt(mainCamera.transform, Vector3.up);
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