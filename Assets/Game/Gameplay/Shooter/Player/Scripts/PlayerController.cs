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

public class PlayerController : MonoBehaviour, IRecieveDamage
{
    [Header("General Settings"), Space]
    [SerializeField] private int lives = 0;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] [Range(.5f, 3f)] private float attackSpeed = 0f;
    [SerializeField] private LayerMask attackLayer = default;

    [Header("Reference Settings"), Space]
    [SerializeField] private Transform body = null;
    [SerializeField] private PlayerLocomotionController locomotionController = null;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private Camera mainCamera = null;

    [Header("Arrow Settings"), Space]
    [SerializeField] private BorrowController arrowController = null;
    [SerializeField] private float arrowForce = 0f;

    [Header("Sounds Settings")]
    [SerializeField] private AudioEvent reloadArrowEvent = null;
    [SerializeField] private AudioEvent fireArrowEvent = null;

    private PlayerInput inputAction = null;

    private Vector2 move = Vector2.zero;
    private float fallVelocity = 0f;
    private bool firePressed = false;
    private Vector3 currentDir = Vector3.zero;

    private int currentLives = 0;

    private Action onDeath = null;

    private const string moveInputKey = "move";
    private const string fireInputKey = "fire";

    private void Awake()
    {
        inputAction = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        locomotionController.Init(attackSpeed, ReloadArrow, FireArrow, onEnableInput: () => UpdateInputFSM(FSM_INPUT.ENABLE_ALL));

        currentLives = lives;
        UpdateInputFSM(FSM_INPUT.ENABLE_ALL);
    }

    private void Update()
    {
        if (CheckIsDead()) return;

        ApplyGravity();
        Movement();
        Attack();
        UpdateRotation();

        locomotionController.UpdateIdleRunAnimation(move.magnitude);
    }

    public void Init(Action onDeath)
    {
        this.onDeath = onDeath;
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

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        firePressed = value.isPressed;
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            fallVelocity = -Physics.gravity.magnitude * mass * Time.deltaTime;
        }
        else
        {
            fallVelocity -= Physics.gravity.magnitude * mass * Time.deltaTime;
        }
    }

    private void Movement()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (move.magnitude > Mathf.Epsilon)
        {
            currentDir = movement;
        }

        movement.y = fallVelocity;

        characterController.Move(movement * Time.deltaTime * moveSpeed);
    }

    private void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, attackLayer, QueryTriggerInteraction.Ignore))
        {
            Vector3 direction = hit.point - body.position;
            direction.y = 0f;
            currentDir = direction;
        }
    }

    private void Attack()
    {
        if (!firePressed) return;
        firePressed = false;

        LookAtMouse();
        locomotionController.PlayAttackAnimation();

        UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
    }

    private void ReloadArrow()
    {
        GameManager.Instance.AudioManager.PlayAudio(reloadArrowEvent);
    }

    private void FireArrow()
    {
        GameManager.Instance.AudioManager.PlayAudio(fireArrowEvent);
        arrowController.FireArrow(arrowForce, body.transform.forward);
    }

    private void UpdateRotation()
    {
        if (currentDir.magnitude > Mathf.Epsilon)
        {
            Quaternion toRot = Quaternion.LookRotation(currentDir, Vector3.up);
            body.rotation = Quaternion.RotateTowards(body.rotation, toRot, rotationSpeed * Time.deltaTime);
        }
    }

    private bool CheckIsDead()
    {
        return currentLives <= 0;
    }

    public void RecieveDamage(int damage)
    {
        currentLives = Mathf.Clamp(currentLives - damage, 0, lives);

        if (CheckIsDead())
        {
            characterController.enabled = false;
            locomotionController.PlayDeadAnimation();
            onDeath?.Invoke();
        }
        else
        {
            locomotionController.PlayRecieveHitAnimation();
            UpdateInputFSM(FSM_INPUT.MOVEMENT);
        }
    }
}
