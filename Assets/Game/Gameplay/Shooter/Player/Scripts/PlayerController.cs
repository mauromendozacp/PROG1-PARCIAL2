using System;

using UnityEngine;

public class PlayerController : MonoBehaviour, IRecieveDamage
{
    [Header("General Settings"), Space]
    [SerializeField] private int lives = 0;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] [Range(.5f, 3f)] private float attackSpeed = 0f;
    [SerializeField] private LayerMask rangeAttackLayer = default;
    [SerializeField] private LayerMask attackObjectsLayer = default;

    [Header("Reference Settings"), Space]
    [SerializeField] private Transform body = null;
    [SerializeField] private PlayerInputController inputController = null;
    [SerializeField] private PlayerLocomotionController locomotionController = null;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private Camera mainCamera = null;

    [Header("Arrow Settings"), Space]
    [SerializeField] private BorrowController arrowController = null;
    [SerializeField] private float arrowForce = 0f;

    [Header("Sounds Settings")]
    [SerializeField] private AudioEvent reloadArrowEvent = null;
    [SerializeField] private AudioEvent fireArrowEvent = null;
    [SerializeField] private AudioEvent hurtEvent = null;
    [SerializeField] private AudioEvent deathEvent = null;

    private float fallVelocity = 0f;
    private Vector3 currentDir = Vector3.zero;

    private int currentLives = 0;
    private bool defeat = false;

    private Action onDeath = null;
    private Action onPause = null;
    private Action<int, int> onUpdateLives = null;

    private void Start()
    {
        inputController.Init(Attack, Pause);
        locomotionController.Init(attackSpeed, ReloadArrow, FireArrow, onEnableInput: () => inputController.UpdateInputFSM(FSM_INPUT.ENABLE_ALL));

        inputController.UpdateInputFSM(FSM_INPUT.ENABLE_ALL);
    }

    private void Update()
    {
        if (CheckIsDead() || defeat) return;

        ApplyGravity();
        Movement();
        UpdateRotation();

        locomotionController.UpdateIdleRunAnimation(inputController.Move.magnitude);
    }

    public void Init(Action<int, int> onUpdateLives, Action onDeath, Action onPause)
    {
        this.onUpdateLives = onUpdateLives;
        this.onDeath = onDeath;
        this.onPause = onPause;

        UpdateLives(lives);
    }

    public void PlayerDefeat()
    {
        defeat = true;
        inputController.UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
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
        Vector3 movement = new Vector3(inputController.Move.x, 0f, inputController.Move.y);

        if (inputController.Move.magnitude > Mathf.Epsilon)
        {
            currentDir = movement;
        }

        movement.y = fallVelocity;

        characterController.Move(movement * Time.deltaTime * moveSpeed);
    }

    private void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rangeAttackLayer, QueryTriggerInteraction.Ignore))
        {
            GameObject hitGO = hit.collider.gameObject;
            Vector3 hitPos = Utils.CheckLayerInMask(attackObjectsLayer, hitGO.layer) ? hitGO.transform.position : hit.point;

            Vector3 direction = hitPos - body.position;
            direction.y = 0f;
            currentDir = direction;
        }
    }

    private void Attack()
    {
        LookAtMouse();
        locomotionController.PlayAttackAnimation();

        inputController.UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
    }

    private void Pause()
    {
        onPause?.Invoke();
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

    private void UpdateLives(int lives)
    {
        currentLives = lives;
        onUpdateLives?.Invoke(currentLives, this.lives);
    }

    public void RecieveDamage(int damage)
    {
        UpdateLives(Mathf.Clamp(currentLives - damage, 0, lives));

        if (CheckIsDead())
        {
            characterController.enabled = false;
            locomotionController.PlayDeadAnimation();
            onDeath?.Invoke();

            GameManager.Instance.AudioManager.PlayAudio(deathEvent);
        }
        else
        {
            locomotionController.PlayRecieveHitAnimation();
            inputController.UpdateInputFSM(FSM_INPUT.MOVEMENT);

            GameManager.Instance.AudioManager.PlayAudio(hurtEvent);
        }
    }
}
