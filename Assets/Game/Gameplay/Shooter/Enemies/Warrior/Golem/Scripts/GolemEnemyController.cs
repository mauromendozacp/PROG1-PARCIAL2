using UnityEngine;

public class GolemEnemyController : WarriorEnemyController
{
    [Header("Attack Distance Settings")]
    [SerializeField] private Rock rock = null;
    [SerializeField] private int rockDamage = 0;
    [SerializeField] private float attackRockDistance = 0f;
    [SerializeField] private float maxHeight = 0f;
    [SerializeField] private Transform handTransform = null;
    [SerializeField] private AudioEvent throwRockEvent = null;

    private GolemLocomotionController golemLocomotionController = null;

    protected override void Awake()
    {
        base.Awake();

        golemLocomotionController = locomotionController as GolemLocomotionController;
        golemLocomotionController.SetOnSpawnRock(SpawnRockToTarget);
        golemLocomotionController.SetOnThrowRock(ThrowRock);

        rock.Init(attackLayer, rockDamage, SetIdleState);
    }

    protected override void UpdateGoToTargetState()
    {
        if (CheckTargetIsOnFront(out RaycastHit hit))
        {
            AttackTarget(hit);
        }
        else
        {
            Transform currentTarget = GetFocusTarget();
            if (currentTarget != null)
            {
                if (Vector3.Distance(currentTarget.position, transform.position) < attackRockDistance)
                {
                    ThrowRockAttack();
                }
                else
                {
                    locomotionController.UpdateIdleRunAnimation(1f);
                    agent.SetDestination(GetFocusTarget().position);
                }
            }
            else
            {
                SetIdleState();
            }
        }
    }

    private void ThrowRockAttack()
    {
        state = FSM_ENEMY.ATTACK;

        golemLocomotionController.ThrowRockAnimation();

        GameManager.Instance.AudioManager.PlayAudio(throwRockEvent);
    }

    private void SpawnRockToTarget()
    {
        rock.transform.SetParent(handTransform);
        rock.transform.localPosition = Vector3.zero;
        rock.Toggle(true);
    }

    private void ThrowRock()
    {
        Transform currentTarget = GetFocusTarget();
        if (currentTarget != null)
        {
            rock.transform.SetParent(null);
            rock.ApplyForce(GetRockVelocity(currentTarget.position));
        }
    }

    private Vector3 GetRockVelocity(Vector3 targetPos)
    {
        Vector3 startPos = rock.transform.position;

        float gravity = Physics.gravity.magnitude;

        Vector3 delta = targetPos - startPos;
        delta.y = 0f;

        float distanceXY = new Vector3(delta.x, 0f, delta.z).magnitude;

        float initialVelY = Mathf.Sqrt(2f * gravity * maxHeight);
        float timeTotal = 2f * initialVelY / gravity;
        float initialVelX = distanceXY / timeTotal;

        Vector3 velocity = delta.normalized * initialVelX;
        velocity.y = initialVelY;

        return velocity;
    }
}
