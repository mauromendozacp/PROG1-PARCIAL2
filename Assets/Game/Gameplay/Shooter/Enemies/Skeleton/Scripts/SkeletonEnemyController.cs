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

public class SkeletonEnemyController : MonoBehaviour, IRecieveDamage
{
    [Header("General Settings"), Space]
    [SerializeField] private int lives = 0;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float distanceToAttack = 0f;
    [SerializeField] private LayerMask attackLayer = default;

    [Header("Reference Settings"), Space]
    [SerializeField] private SkeletonLocomotionController locomotionController = null;
    [SerializeField] private Transform bodyCenterTransform = null;

    private CapsuleCollider capsuleCollider = null;
    private NavMeshAgent agent = null;
    [SerializeField] private Transform target = null;

    private FSM_ENEMY state = default;
    private int currentLives = 0;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        locomotionController.Init(SetIdleState);
    }

    private void Update()
    {
        UpdateFSM();
    }

    public void Init(Transform target)
    {
        this.target = target;
    }

    public void Spawn()
    {
        currentLives = lives;
        ToggleCollider(true);
        locomotionController.PlayIdleRunAnimation();
        state = FSM_ENEMY.IDLE;
    }

    private void UpdateFSM()
    {
        switch (state)
        {
            case FSM_ENEMY.IDLE:
                locomotionController.UpdateIdleRunAnimation(0f);
                if (target != null)
                {
                    state = FSM_ENEMY.GO_TO_TARGET;
                }

                break;
            case FSM_ENEMY.GO_TO_TARGET:
                if (CheckPlayerIsOnFront())
                {
                    locomotionController.PlayAttackAnimation();
                    state = FSM_ENEMY.ATTACK;
                }
                else
                {
                    locomotionController.UpdateIdleRunAnimation(1f);
                    agent.SetDestination(target.position);
                }

                break;
            case FSM_ENEMY.ATTACK:
                break;
            case FSM_ENEMY.RECIEVE_DAMAGE:
                break;
            case FSM_ENEMY.DEATH:
                break;
        }

        agent.isStopped = state != FSM_ENEMY.GO_TO_TARGET;
    }

    private bool CheckPlayerIsOnFront()
    {
        return Physics.Raycast(bodyCenterTransform.position, bodyCenterTransform.forward, distanceToAttack, attackLayer);
    }

    private void SetIdleState()
    {
        state = FSM_ENEMY.IDLE;
    }

    private void ToggleCollider(bool status)
    {
        capsuleCollider.enabled = status;
    }

    public void RecieveDamage(int damage)
    {
        if (state == FSM_ENEMY.DEATH) return;

        currentLives = Mathf.Clamp(currentLives - damage, 0, lives);

        if (currentLives <= 0)
        {
            locomotionController.PlayDeadAnimation();
            ToggleCollider(false);
            state = FSM_ENEMY.DEATH;
        }
        else
        {
            locomotionController.PlayRecieveHitAnimation();
            state = FSM_ENEMY.RECIEVE_DAMAGE;
        }
    }
}
