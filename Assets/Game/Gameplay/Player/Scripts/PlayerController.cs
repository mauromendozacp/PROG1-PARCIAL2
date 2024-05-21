using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private float mass = 1f;

    private CharacterController characterController = null;

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
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), fallVelocity, Input.GetAxis("Vertical"));

        characterController.Move(movement * Time.deltaTime * speed);
    }
}
