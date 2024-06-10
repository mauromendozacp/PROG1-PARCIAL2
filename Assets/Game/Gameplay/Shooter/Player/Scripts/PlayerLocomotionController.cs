using System;

using UnityEngine;

public class PlayerLocomotionController : MonoBehaviour
{
    private const string deadAnimName = "Death";

    private const string moveSpeedKey = "MoveSpeed";
    private const string attackKey = "Attack";
    private const string recieveHitKey = "RecieveHit";
    private const string attackSpeedKey = "AttackSpeed";
    private const string rollKey = "Roll";

    private Animator animator = null;
    private Action onReloadArrow = null;
    private Action onFireArrow = null;
    private Action onEnableInput = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(float attackSpeed, Action onReloadArrow, Action onFireArrow, Action onEnableInput)
    {
        animator.SetFloat(attackSpeedKey, attackSpeed);

        this.onReloadArrow = onReloadArrow;
        this.onFireArrow = onFireArrow;
        this.onEnableInput = onEnableInput;
    }

    public void UpdateIdleRunAnimation(float speed)
    {
        animator.SetFloat(moveSpeedKey, speed);
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

    public void PlayRollAnimation()
    {
        animator.SetTrigger(rollKey);
    }

    public void ReloadArrow()
    {
        onReloadArrow?.Invoke();
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
