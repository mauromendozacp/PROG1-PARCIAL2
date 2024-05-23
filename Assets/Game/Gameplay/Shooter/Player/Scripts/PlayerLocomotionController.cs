using System;

using UnityEngine;

public class PlayerLocomotionController : MonoBehaviour
{
    private const string speedKey = "Speed";
    private const string attackKey = "Attack";
    private const string deadKey = "Death";
    private const string recieveHitKey = "RecieveHit";

    private Animator animator = null;
    private Action onFireArrow = null;
    private Action onEnableInput = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Action onFireArrow, Action onEnableInput)
    {
        this.onFireArrow = onFireArrow;
        this.onEnableInput = onEnableInput;
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger(attackKey);
    }

    public void FireArrow()
    {
        onFireArrow?.Invoke();
    }

    public void EnableInput()
    {
        onEnableInput?.Invoke();
    }

    public void UpdateIdleRunAnimation(float speed)
    {
        animator.SetFloat(speedKey, speed);
    }

    public void PlayDeadAnimation()
    {
        animator.Play(deadKey, 0);
        animator.Play(deadKey, 1);
    }

    public void PlayRecieveHitAnimation()
    {
        animator.SetTrigger(recieveHitKey);
    }
}
