using UnityEngine;

public class SkeletonEnemyController : EnemyController, IRecieveDamage
{
    [Header("Reference Settings"), Space]
    [SerializeField] private SkeletonLocomotionController locomotionController = null;
    [SerializeField] private Transform bodyCenterTransform = null;

    private CapsuleCollider capsuleCollider = null;

    protected override void Awake()
    {
        base.Awake();

        capsuleCollider = GetComponent<CapsuleCollider>();

        locomotionController.Init(SetIdleState);
    }

    protected override void UpdateFSM()
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
                if (CheckPlayerIsOnFront(out RaycastHit hit))
                {
                    AttackTarget(hit);
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

    private bool CheckPlayerIsOnFront(out RaycastHit hit)
    {
        if (Physics.Raycast(bodyCenterTransform.position, bodyCenterTransform.forward, out RaycastHit hitInfo, distanceToAttack, attackLayer))
        {
            hit = hitInfo;
            return true;
        }

        hit = default;
        return false;
    }

    private void SetIdleState()
    {
        state = FSM_ENEMY.IDLE;
    }

    private void ToggleCollider(bool status)
    {
        capsuleCollider.enabled = status;
    }

    private void AttackTarget(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent(out IRecieveDamage recieveDamage))
        {
            recieveDamage.RecieveDamage(damage);

            locomotionController.PlayAttackAnimation();
            state = FSM_ENEMY.ATTACK;
        }
        else
        {
            Debug.LogError("Can't attack this object: " + hit.collider.gameObject.name);
        }
    }

    public override void RecieveDamage(int damage)
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

    public override void OnGet()
    {
        ToggleCollider(true);
        locomotionController.PlayIdleRunAnimation();

        base.OnGet();
    }
}
