using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("General Settings"), Space]
    [SerializeField] private int lives = 0;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float mass = 1f;

    [Header("Reference Settings"), Space]
    [SerializeField] private Transform body = null;
    [SerializeField] private PlayerLocomotionController locomotionController = null;

    [Header("Arrow Settings"), Space]
    [SerializeField] private ArrowController arrowController = null;
    [SerializeField] private float arrowForce = 0f;

    private PlayerInput inputAction = null;
    private CharacterController characterController = null;
    private Camera mainCamera = null;

    private Vector2 move = Vector2.zero;
    private float fallVelocity = 0f;
    private bool firePressed = false;

    private int currentLives = 0;
    private bool isDead = false;

    private void Awake()
    {
        inputAction = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        locomotionController.Init(FireArrow, FinishFire);

        currentLives = lives;
    }

    private void Update()
    {
        if (isDead) return;

        ApplyGravity();
        Movement();
        Attack();

        locomotionController.UpdateIdleRunAnimation(move.magnitude);

        CheckLives();
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
            Vector3 direction = hit.point - transform.position;
            direction.y = 0f;
            body.forward = direction;
        }
    }

    private void Attack()
    {
        if (!firePressed) return;

        LookAtMouse();
        locomotionController.Attack();

        firePressed = false;
        inputAction.DeactivateInput();
    }

    private void FireArrow()
    {
        arrowController.FireArrow(arrowForce, body.transform.forward);
    }

    private void FinishFire()
    {
        inputAction.ActivateInput();
    }

    private void CheckLives()
    {
        if (currentLives <= 0)
        {
            isDead = true;
        }
    }
}
