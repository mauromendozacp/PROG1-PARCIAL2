using System;

using UnityEngine;

public class PlayerLocomotionController : MonoBehaviour
{
    private const string deadAnimName = "Death";

    private const string speedKey = "Speed";
    private const string attackKey = "Attack";
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

    public void UpdateIdleRunAnimation(float speed)
    {
        animator.SetFloat(speedKey, speed);
    }

    public void PlayDeadAnimation()
    {
        animator.Play(deadAnimName, 0);
        animator.Play(deadAnimName, 1);
    }

    public void PlayRecieveHitAnimation()
    {
        animator.SetTrigger(recieveHitKey);
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
}
