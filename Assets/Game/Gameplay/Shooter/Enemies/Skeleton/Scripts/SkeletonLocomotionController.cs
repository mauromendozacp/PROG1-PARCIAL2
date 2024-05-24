using System;

using UnityEngine;

public class SkeletonLocomotionController : MonoBehaviour
{
    private const string idleRunAnimName = "Idle/Run";
    private const string deadAnimName = "Death";

    private const string speedKey = "Speed";
    private const string attackKey = "Attack";
    private const string recieveHitKey = "RecieveHit";

    private Animator animator = null;
    private Action onSetIdleState = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Action onSetIdleState)
    {
        this.onSetIdleState = onSetIdleState;
    }

    public void SetIdleState()
    {
        onSetIdleState?.Invoke();
    }

    public void PlayIdleRunAnimation()
    {
        animator.Play(idleRunAnimName);
    }

    public void UpdateIdleRunAnimation(float speed)
    {
        animator.SetFloat(speedKey, speed);
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger(attackKey);
    }

    public void PlayDeadAnimation()
    {
        animator.Play(deadAnimName);
    }

    public void PlayRecieveHitAnimation()
    {
        animator.SetTrigger(recieveHitKey);
    }
}
