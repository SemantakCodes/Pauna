using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float crouchSpeed = 2.5f;
    [SerializeField] public float gravity = 20f;

    [Header("Camera")]
    [SerializeField] public Camera playerCamera;
    [SerializeField] public float lookSpeed = 2f;

    [Header("Crouch")]
    [SerializeField] private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    [SerializeField] private Vector3 standScale = new Vector3(1, 1, 1);

    [Header("Input (Set from Joystick / Touch)")]
    [SerializeField] public Vector2 moveInput;
    [SerializeField] public Vector2 lookInput;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float rotationX = 0;

    [SerializeField] private bool isCrouching = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        MouseLook();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float speed = isCrouching ? crouchSpeed : walkSpeed;

        Vector3 move = (forward * moveInput.y + right * moveInput.x) * speed;

        moveDirection.x = move.x;
        moveDirection.z = move.z;

        controller.Move(moveDirection * Time.deltaTime);
    }
    private void MouseLook()
    {
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        //  Increase sensitivity for mobile
        float sensitivityMultiplier = 5f;

        rotationX -= mouseY * lookSpeed * sensitivityMultiplier;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX * lookSpeed * sensitivityMultiplier);
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -1f; // keeps grounded
        }
    }

    // =========================
    // 🎮 HORROR GAME FEATURES
    // =========================

    // Walking toggle (slow pacing horror style)
    public void SetWalking(bool walking)
    {
        walkSpeed = walking ? 5f : 2.5f;
    }

    // Crouch system
    public void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            transform.localScale = crouchScale;
        }
        else
        {
            transform.localScale = standScale;
        }
    }

    // =========================
    // 🧠 FUTURE SYSTEMS (PLACEHOLDERS)
    // =========================

    // Interaction system (doors, items, etc.)
    public void Interact()
    {
        Debug.Log("Interact triggered");
        // TODO: Raycast + interactable objects
    }

    // Mobile touch input hookup
    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }
}