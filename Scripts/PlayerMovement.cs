using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference sprintAction;
    public InputActionReference crouchAction;

    [Header("Camera Setup")]
    [Tooltip("Create an empty GameObject as a child of the player, name it 'CameraRoot', and assign it here. Put the actual camera inside that root.")]
    public Transform cameraRoot; 
    public float mouseSensitivity = 15f;
    public float lookSmoothTime = 0.05f;
    public bool invertY = false;

    [Header("Movement Settings")]
    public float walkSpeed = 2.5f;
    public float sprintSpeed = 5.0f;
    public float crouchSpeed = 1.5f;
    public float moveSmoothTime = 0.15f;
    public float gravity = -9.81f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    public float currentStamina { get; private set; }

    [Header("Crouch Settings")]
    public float standHeight = 2.0f;
    public float crouchHeight = 1.0f;
    public float crouchSmoothTime = 0.15f;

    // Public States for the Camera Script to read
    public bool IsMoving { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsCrouching { get; private set; }

    // Internal Variables
    private CharacterController controller;
    private Vector2 currentLookInput;
    private Vector2 lookVelocity;
    private float cameraPitch = 0f;
    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;
    private Vector3 playerVelocity;
    private float currentHeight;
    private float heightVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
        currentHeight = standHeight;

        moveAction.action.Enable();
        lookAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
    }

    private void Update()
    {
        HandleInput();
        HandleLook();
        HandleMovement();
        HandleStamina();
        HandleCrouch();
    }

    private void HandleInput()
    {
        Vector2 inputDir = moveAction.action.ReadValue<Vector2>();
        IsMoving = inputDir.magnitude > 0.1f;

        // 1 & 2: Crouch Logic (Works in idle, cancels sprint)
        if (crouchAction.action.WasPressedThisFrame())
        {
            IsCrouching = !IsCrouching;
            if (IsCrouching) IsSprinting = false; 
        }

        // Sprint Logic (Only if not crouching, moving, and has stamina)
        bool sprintInput = sprintAction.action.IsPressed();
        if (sprintInput && !IsCrouching && IsMoving && currentStamina > 0)
        {
            IsSprinting = true;
        }
        else
        {
            IsSprinting = false;
        }
    }

    private void HandleLook()
    {
        Vector2 targetLookInput = lookAction.action.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;
        currentLookInput = Vector2.SmoothDamp(currentLookInput, targetLookInput, ref lookVelocity, lookSmoothTime);

        cameraPitch -= currentLookInput.y * (invertY ? -1 : 1);
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);

        // Rotate the root up/down, rotate the player left/right
        cameraRoot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * currentLookInput.x);
    }

    private void HandleMovement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; 
        }

        Vector2 inputDir = moveAction.action.ReadValue<Vector2>();
        Vector3 targetDirection = transform.right * inputDir.x + transform.forward * inputDir.y;
        targetDirection.Normalize();

        float targetSpeed = IsCrouching ? crouchSpeed : (IsSprinting ? sprintSpeed : walkSpeed);
        
        currentMoveVelocity = Vector3.SmoothDamp(currentMoveVelocity, targetDirection * targetSpeed, ref moveDampVelocity, moveSmoothTime);
        controller.Move(currentMoveVelocity * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleStamina()
    {
        if (IsSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina);
        }
    }

    private void HandleCrouch()
    {
        float targetHeight = IsCrouching ? crouchHeight : standHeight;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, crouchSmoothTime);
        
        controller.height = currentHeight;
        controller.center = new Vector3(0, currentHeight / 2, 0); 
        
        // Keep the CameraRoot near the top of the character controller dynamically
        cameraRoot.localPosition = new Vector3(0, currentHeight - 0.2f, 0);
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
    }
}
