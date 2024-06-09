using UnityEngine;

public class WarriorEnemyController : EnemyController, IRecieveDamage
{
    [Header("Reference Settings"), Space]
    [SerializeField] protected WarriorLocomotionController locomotionController = null;
    [SerializeField] protected Transform bodyCenterTransform = null;

    protected CapsuleCollider capsuleCollider = null;

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
                UpdateIdleState();

                break;
            case FSM_ENEMY.GO_TO_TARGET:
                UpdateGoToTargetState();

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

    protected virtual void UpdateIdleState()
    {
        locomotionController.UpdateIdleRunAnimation(0f);
        if (GetFocusTarget() != null)
        {
            state = FSM_ENEMY.GO_TO_TARGET;
        }
    }

    protected virtual void UpdateGoToTargetState()
    {
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
                SetIdleState();
            }
        }
    }

    protected bool CheckTargetIsOnFront(out RaycastHit hit)
    {
        if (Physics.Raycast(bodyCenterTransform.position, bodyCenterTransform.forward, out RaycastHit hitInfo, distanceToAttack, attackLayer))
        {
            hit = hitInfo;
            return true;
        }

        hit = default;
        return false;
    }

    protected void SetIdleState()
    {
        state = FSM_ENEMY.IDLE;
    }

    protected void AttackTarget(RaycastHit hit)
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

    private void ToggleCollider(bool status)
    {
        capsuleCollider.enabled = status;
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
