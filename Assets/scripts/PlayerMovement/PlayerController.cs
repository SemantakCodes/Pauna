using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float gravity = 20f;

    [Header("Camera")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Crouch")]
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 standScale = new Vector3(1, 1, 1);

    [Header("Input (Set from Joystick / Touch)")]
    public Vector2 moveInput;
    public Vector2 lookInput;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    private bool isCrouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        ApplyGravity();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float speed = isCrouching ? crouchSpeed : walkSpeed;

        Vector3 move = (forward * moveInput.y + right * moveInput.x) * speed;

        moveDirection.x = move.x;
        moveDirection.z = move.z;

        controller.Move(moveDirection * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // For testing (PC)
        float mouseX = lookInput.x != 0 ? lookInput.x : Input.GetAxis("Mouse X");
        float mouseY = lookInput.y != 0 ? lookInput.y : Input.GetAxis("Mouse Y");

        rotationX -= mouseY * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX * lookSpeed);
    }

    void ApplyGravity()
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