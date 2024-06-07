using System;

using UnityEngine;
using UnityEngine.InputSystem;

public enum FSM_INPUT
{
    ENABLE_ALL,
    MOVEMENT,
    ATTACK,
    DISABLE_ALL
}

public class PlayerInputController : MonoBehaviour
{
    private PlayerInput inputAction = null;

    private Vector2 move = Vector2.zero;

    private Action onFire = null;
    private Action onPause = null;

    private const string moveInputKey = "move";
    private const string fireInputKey = "fire";

    public Vector2 Move { get => move; }

    private void Awake()
    {
        inputAction = GetComponent<PlayerInput>();
    }

    public void Init(Action onFire, Action onPause)
    {
        this.onFire = onFire;
        this.onPause = onPause;
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            onFire?.Invoke();
        }
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            onPause?.Invoke();
        }
    }

    public void UpdateInputFSM(FSM_INPUT fsm)
    {
        switch (fsm)
        {
            case FSM_INPUT.ENABLE_ALL:
                inputAction.actions[moveInputKey].Enable();
                inputAction.actions[fireInputKey].Enable();
                break;
            case FSM_INPUT.MOVEMENT:
                inputAction.actions[moveInputKey].Enable();
                inputAction.actions[fireInputKey].Disable();
                break;
            case FSM_INPUT.ATTACK:
                inputAction.actions[moveInputKey].Disable();
                inputAction.actions[fireInputKey].Enable();
                break;
            case FSM_INPUT.DISABLE_ALL:
                inputAction.actions[moveInputKey].Disable();
                inputAction.actions[fireInputKey].Disable();
                break;
        }
    }
}
