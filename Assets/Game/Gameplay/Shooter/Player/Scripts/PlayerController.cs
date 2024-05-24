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
    [SerializeField] private float speed = 0f;
    [SerializeField] private float mass = 1f;

    [Header("Reference Settings"), Space]
    [SerializeField] private Transform body = null;
    [SerializeField] private PlayerLocomotionController locomotionController = null;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private Camera mainCamera = null;

    [Header("Arrow Settings"), Space]
    [SerializeField] private ArrowController arrowController = null;
    [SerializeField] private float arrowForce = 0f;

    private PlayerInput inputAction = null;

    private Vector2 move = Vector2.zero;
    private float fallVelocity = 0f;
    private bool firePressed = false;

    private int currentLives = 0;
    private bool isDead = false;

    private const string moveInputKey = "move";
    private const string fireInputKey = "fire";

    private void Awake()
    {
        inputAction = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        locomotionController.Init(FireArrow, onEnableInput: () => UpdateInputFSM(FSM_INPUT.ENABLE_ALL));

        currentLives = lives;
        UpdateInputFSM(FSM_INPUT.ENABLE_ALL);
    }

    private void Update()
    {
        if (isDead) return;

        ApplyGravity();
        Movement();
        Attack();

        locomotionController.UpdateIdleRunAnimation(move.magnitude);
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
            body.forward = movement;
        }

        movement.y = fallVelocity;

        characterController.Move(movement * Time.deltaTime * speed);
    }

    private void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 direction = hit.point - body.position;
            direction.y = 0f;
            body.forward = direction;
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

    private void FireArrow()
    {
        arrowController.FireArrow(arrowForce, body.transform.forward);
    }

    private void UpdateInputFSM(FSM_INPUT fsm)
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

    public void RecieveDamage(int damage)
    {
        currentLives = Mathf.Clamp(currentLives - damage, 0, lives);

        if (currentLives <= 0)
        {
            isDead = true;
            locomotionController.PlayDeadAnimation();
            UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
        }
        else
        {
            locomotionController.PlayRecieveHitAnimation();
            UpdateInputFSM(FSM_INPUT.MOVEMENT);
        }
    }
}
