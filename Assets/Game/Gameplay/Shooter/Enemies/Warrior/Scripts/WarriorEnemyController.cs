using UnityEngine;

public class WarriorEnemyController : EnemyController, IRecieveDamage
{
    [Header("Reference Settings"), Space]
    [SerializeField] private WarriorLocomotionController locomotionController = null;
    [SerializeField] private Transform bodyCenterTransform = null;

    private CapsuleCollider capsuleCollider = null;

    protected override void Awake()
    {
        base.Awake();

        capsuleCollider = GetComponent<CapsuleCollider>();

        locomotionController.Init(SetIdleState, onFinishDeath: () => onRelease?.Invoke(this));
    }

    protected override void UpdateFSM()
    {
        switch (state)
        {
            case FSM_ENEMY.IDLE:
                locomotionController.UpdateIdleRunAnimation(0f);
                if (GetFocusTarget() != null)
                {
                    state = FSM_ENEMY.GO_TO_TARGET;
                }

                break;
            case FSM_ENEMY.GO_TO_TARGET:
                if (CheckTargetIsOnFront(out RaycastHit hit))
                {
                    AttackTarget(hit);
                }
                else
                {
                    if (GetFocusTarget() != null)
                    {
                        locomotionController.UpdateIdleRunAnimation(1f);
                        agent.SetDestination(GetFocusTarget().position);
                    }
                    else
                    {
                        state = FSM_ENEMY.IDLE;
                    }
                }

                break;
            case FSM_ENEMY.ATTACK:
                break;
            case FSM_ENEMY.HURT:
                break;
            case FSM_ENEMY.DEATH:
                break;
            case FSM_ENEMY.WIN:
                break;
        }

        agent.isStopped = state != FSM_ENEMY.GO_TO_TARGET;
    }

    private bool CheckTargetIsOnFront(out RaycastHit hit)
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

            Attack();
        }
        else
        {
            Debug.LogWarning("Can't attack this object: " + hit.collider.gameObject.name);
        }
    }

    public override void SetWinState()
    {
        base.SetWinState();

        locomotionController.PlayIdleRunAnimation();
    }

    protected override void Attack()
    {
        base.Attack();

        locomotionController.PlayAttackAnimation();
    }

    protected override void Hurt()
    {
        base.Hurt();

        locomotionController.PlayRecieveHitAnimation();
    }

    protected override void Death()
    {
        base.Death();

        locomotionController.PlayDeadAnimation();
        ToggleCollider(false);
    }

    public override void RecieveDamage(int damage)
    {
        if (state == FSM_ENEMY.DEATH) return;

        UpdateLife(Mathf.Clamp(currentLives - damage, 0, lives));

        if (currentLives <= 0)
        {
            Death();
        }
        else
        {
            Hurt();
        }
    }

    public override void OnGet()
    {
        UpdateLife(lives);
        ToggleCollider(true);
        locomotionController.PlayIdleRunAnimation();

        base.OnGet();
    }
}
