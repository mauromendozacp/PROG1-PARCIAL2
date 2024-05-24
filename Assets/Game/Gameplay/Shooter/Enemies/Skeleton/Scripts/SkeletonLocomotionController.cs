using System;

using UnityEngine;

public class SkeletonLocomotionController : MonoBehaviour
{
    private const string speedKey = "Speed";
    private const string attackKey = "Attack";
    private const string deadKey = "Death";
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

    public void PlayAttackAnimation()
    {
        animator.SetTrigger(attackKey);
    }

    public void SetIdleState()
    {
        onSetIdleState?.Invoke();
    }

    public void UpdateIdleRunAnimation(float speed)
    {
        animator.SetFloat(speedKey, speed);
    }

    public void PlayDeadAnimation()
    {
        animator.Play(deadKey);
    }

    public void PlayRecieveHitAnimation()
    {
        animator.SetTrigger(recieveHitKey);
    }
}
