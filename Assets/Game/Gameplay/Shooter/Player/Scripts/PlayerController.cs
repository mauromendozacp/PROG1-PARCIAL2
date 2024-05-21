using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private float mass = 1f;

    private CharacterController characterController = null;

    private Vector2 move = Vector2.zero;

    private float fallVelocity = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();
        Movement();
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
        Vector3 movement = new Vector3(move.x, fallVelocity, move.y);

        characterController.Move(movement * Time.deltaTime * speed);
    }
}
