using System;

using UnityEngine;

public class PlayerLocomotionController : MonoBehaviour
{
    private const string speedKey = "Speed";
    private const string attackKey = "Attack";

    private Animator animator = null;
    private Action onFireArrow = null;
    private Action onFinishFire = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Action onFireArrow, Action onFinishFire)
    {
        this.onFireArrow = onFireArrow;
        this.onFinishFire = onFinishFire;
    }

    public void Attack()
    {
        animator.SetTrigger(attackKey);
    }

    public void FireArrow()
    {
        onFireArrow?.Invoke();
    }

    public void FinishFire()
    {
        onFinishFire?.Invoke();
    }

    public void UpdateIdleRunAnimation(float speed)
    {
        animator.SetFloat(speedKey, speed);
    }
}
