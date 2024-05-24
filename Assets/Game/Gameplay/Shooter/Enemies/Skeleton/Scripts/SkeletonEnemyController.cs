using UnityEngine;
using UnityEngine.AI;

public enum FSM_ENEMY
{
    IDLE,
    GO_TO_TARGET,
    ATTACK,
    DEATH
}

public class SkeletonEnemyController : MonoBehaviour
{
    [Header("General Settings"), Space]
    [SerializeField] private float speed = 0f;
    [SerializeField] private float distanceToAttack = 0f;
    [SerializeField] private LayerMask attackLayer = default;

    [Header("Reference Settings"), Space]
    [SerializeField] private SkeletonLocomotionController locomotionController = null;
    [SerializeField] private Transform bodyCenterTransform = null;
    
    private NavMeshAgent agent = null;

    private FSM_ENEMY state = default;
    [SerializeField] private Transform target = null;

    private void Awake()
    {
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
}
