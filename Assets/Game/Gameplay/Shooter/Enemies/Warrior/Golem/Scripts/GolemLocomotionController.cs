using System;

public class GolemLocomotionController : WarriorLocomotionController
{
    private const string throwRockKey = "ThrowRock";

    private Action onSpawnRock = null;
    private Action onThrowRock = null;

    public void SetOnSpawnRock(Action onSpawnRock)
    {
        this.onSpawnRock = onSpawnRock;
    }

    public void SetOnThrowRock(Action onThrowRock)
    {
        this.onThrowRock = onThrowRock;
    }

    public void ThrowRockAnimation()
    {
        animator.SetTrigger(throwRockKey);
    }

    public void SpawnRock()
    {
        onSpawnRock?.Invoke();
    }

    public void ThrowRock()
    {
        onThrowRock?.Invoke();
    }
}
