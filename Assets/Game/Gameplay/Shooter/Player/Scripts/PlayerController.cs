using System;
using System.Collections;

using UnityEngine;

public class PlayerController : MonoBehaviour, IRecieveDamage
{
    [Header("General Settings"), Space]
    [SerializeField] private int lives = 0;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] private bool debug = false;

    [Header("Attack Settings"), Space]
    [SerializeField] [Range(.5f, 3f)] private float attackSpeed = 0f;
    [SerializeField] private LayerMask rangeAttackLayer = default;
    [SerializeField] private LayerMask attackObjectsLayer = default;

    [Header("Arrow Settings"), Space]
    [SerializeField] private BorrowController arrowController = null;
    [SerializeField] private float arrowForce = 0f;

    [Header("Roll Settings"), Space]
    [SerializeField] private AnimationCurve rollCurve = null;
    [SerializeField] private float rollSpeed = 0f;
    [SerializeField] private float rollDuration = 0f;

    [Header("Reference Settings"), Space]
    [SerializeField] private Transform body = null;
    [SerializeField] private PlayerInputController inputController = null;
    [SerializeField] private PlayerLocomotionController locomotionController = null;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private Camera mainCamera = null;

    [Header("Sounds Settings")]
    [SerializeField] private AudioEvent reloadArrowEvent = null;
    [SerializeField] private AudioEvent fireArrowEvent = null;
    [SerializeField] private AudioEvent hurtEvent = null;
    [SerializeField] private AudioEvent deathEvent = null;

    private float fallVelocity = 0f;
    private Vector3 currentDir = Vector3.zero;

    private int currentLives = 0;
    private float currentMoveSpeed = 0f;

    private bool invinsible = false;

    private IEnumerator increaseAttackSpeedCoroutine = null;

    private Action onDeath = null;
    private Action onPause = null;
    private Action<int, int> onUpdateLives = null;

    private void Start()
    {
        if (debug)
        {
            Setup();
        }
    }

    private void Update()
    {
        if (CheckIsDead()) return;

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

        Setup();
        UpdateLives(lives);

        inputController.UpdateInputFSM(FSM_INPUT.DISABLE_ALL);
    }

    public void PlayerDefeat()
    {
        EnableInputOnlyUI();
        locomotionController.UpdateIdleRunAnimation(0f);
    }

    public void EnableInput(bool force = false)
    {
        if (!force && CheckDisableInputStates()) return;

        inputController.UpdateInputFSM(FSM_INPUT.ENABLE_ALL);
    }

    public void EnableInputOnlyUI()
    {
        inputController.UpdateInputFSM(FSM_INPUT.ONLY_UI);
    }

    public void IncreaseLives(int increaseLives)
    {
        UpdateLives(currentLives + increaseLives);
    }

    public void IncreaseAttackSpeed(float increaseSpeedPorc, float duration)
    {
        if (increaseAttackSpeedCoroutine != null)
        {
            StopCoroutine(increaseAttackSpeedCoroutine);
        }

        increaseAttackSpeedCoroutine = IncreaseAttackSpeedCoroutine();
        StartCoroutine(increaseAttackSpeedCoroutine);

        IEnumerator IncreaseAttackSpeedCoroutine()
        {
            locomotionController.SetAttackSpeed(attackSpeed * (increaseSpeedPorc + 100) / 100);

            yield return new WaitForSeconds(duration);

            locomotionController.SetAttackSpeed(attackSpeed);
            increaseAttackSpeedCoroutine = null;
        }
    }

    private void Setup()
    {
        inputController.Init(Attack, Roll, Pause);
        locomotionController.Init(attackSpeed, ReloadArrow, FireArrow, onEnableInput: () => EnableInput());

        currentLives = lives;
        currentMoveSpeed = moveSpeed;
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

        characterController.Move(movement * Time.deltaTime * currentMoveSpeed);
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

        inputController.UpdateInputFSM(FSM_INPUT.DISABLE_INTERACTIONS);
    }

    private void Roll()
    {
        if (inputController.Move.magnitude < Mathf.Epsilon) return;
        Vector3 moveDir = new Vector3(inputController.Move.x, 0f, inputController.Move.y);

        locomotionController.PlayRollAnimation();
        inputController.UpdateInputFSM(FSM_INPUT.DISABLE_INTERACTIONS);

        StartCoroutine(RollCoroutine());
        IEnumerator RollCoroutine()
        {
            float timer = 0f;

            while (timer < rollDuration)
            {
                timer += Time.deltaTime;
                Vector3 move = moveDir * rollCurve.Evaluate(timer / rollDuration) * rollSpeed * Time.deltaTime;

                characterController.Move(move);

                yield return new WaitForEndOfFrame();
            }

            EnableInput();
        }
    }

    private void Pause()
    {
        GameManager.Instance.AudioManager.ToggleMusic(false);

        EnableInputOnlyUI();
        onPause?.Invoke();
    }

    private void ReloadArrow()
    {
        GameManager.Instance.AudioManager.PlayAudio(reloadArrowEvent, transform.position);
    }

    private void FireArrow()
    {
        GameManager.Instance.AudioManager.PlayAudio(fireArrowEvent, transform.position);
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

    private void UpdateLives(int newLives)
    {
        currentLives = Mathf.Clamp(newLives, 0, lives);
        onUpdateLives?.Invoke(currentLives, lives);
    }

    private bool CheckDisableInputStates()
    {
        return inputController.CurrentInputState == FSM_INPUT.ONLY_UI || inputController.CurrentInputState == FSM_INPUT.DISABLE_ALL;
    }

    public void RecieveDamage(int damage)
    {
        if (invinsible) return;

        UpdateLives(currentLives - damage);

        if (CheckIsDead())
        {
            characterController.enabled = false;
            locomotionController.PlayDeadAnimation();
            onDeath?.Invoke();

            GameManager.Instance.AudioManager.PlayAudio(deathEvent, transform.position);
        }
        else
        {
            locomotionController.PlayRecieveHitAnimation();
            inputController.UpdateInputFSM(FSM_INPUT.MOVEMENT);

            GameManager.Instance.AudioManager.PlayAudio(hurtEvent, transform.position);
        }
    }
}
