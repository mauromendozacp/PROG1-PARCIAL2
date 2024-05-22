using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform body = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private LayerMask groundLayer = default;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float mass = 1f;

    private CharacterController characterController = null;
    private Camera mainCamera = null;
    private Vector2 move = Vector2.zero;
    private float fallVelocity = 0f;

    private const string speedKey = "Speed";

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ApplyGravity();
        Movement();
        LookAtMouse();

        UpdateAnimations();
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
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
        movement = movement.x * body.right + movement.z * body.forward;
        movement.y = fallVelocity;

        characterController.Move(movement * Time.deltaTime * speed);
    }

    private void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 direction = hit.point - transform.position;
            direction.y = 0f;
            body.forward = direction;
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat(speedKey, move.magnitude);
    }
}
