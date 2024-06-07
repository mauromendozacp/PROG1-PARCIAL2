using System;

using UnityEngine;
using UnityEngine.InputSystem;

public enum FSM_INPUT
{
    ENABLE_ALL,
    MOVEMENT,
    ATTACK,
    ONLY_UI,
    DISABLE_ALL
}

public class PlayerInputController : MonoBehaviour
{
    private PlayerInputActions inputAction = null;

    private Action onFire = null;
    private Action onPause = null;

    public Vector2 Move { get => GetMoveValue(); }

    public void Init(Action onFire, Action onPause)
    {
        this.onFire = onFire;
        this.onPause = onPause;

        inputAction = new PlayerInputActions();

        inputAction.Player.Fire.performed += OnFire;
        inputAction.UI.Pause.performed += OnPause;

        UpdateInputFSM(FSM_INPUT.ENABLE_ALL);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        onFire?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        onPause?.Invoke();
    }

    public void UpdateInputFSM(FSM_INPUT fsm)
    {
        switch (fsm)
        {
            case FSM_INPUT.ENABLE_ALL:
                inputAction.Player.Enable();
                inputAction.UI.Enable();
                break;
            case FSM_INPUT.MOVEMENT:
                inputAction.Player.Fire.Disable();
                inputAction.Player.Move.Enable();
                break;
            case FSM_INPUT.ATTACK:
                inputAction.Player.Fire.Enable();
                inputAction.Player.Move.Disable();
                break;
            case FSM_INPUT.ONLY_UI:
                inputAction.Player.Disable();
                inputAction.UI.Enable();
                break;
            case FSM_INPUT.DISABLE_ALL:
                inputAction.Player.Disable();
                inputAction.UI.Disable();
                break;
        }
    }

    private Vector2 GetMoveValue()
    {
        if (inputAction == null) return Vector2.zero;

        return inputAction.Player.Move.ReadValue<Vector2>();
    }
}