using UnityEngine;

public class HorrorCameraEffects : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement controller;

    [Header("Bob Frequencies")]
    public float idleFrequency = 1.5f;
    public float walkFrequency = 5.0f;
    public float sprintFrequency = 8.0f;
    public float crouchFrequency = 3.5f;

    [Header("Bob Amplitudes")]
    public float idleAmplitude = 0.01f;
    public float walkAmplitude = 0.05f;
    public float sprintAmplitude = 0.12f;
    public float crouchAmplitude = 0.03f;

    [Header("Sway & Tilt")]
    public float swayAmount = 1.5f; // Slight rotation when walking
    public float positionalSmoothTime = 5f;
    public float rotationalSmoothTime = 5f;

    private float timer = 0f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        HandleHeadBob();
    }

    private void HandleHeadBob()
    {
        // 1. Determine current state
        float frequency;
        float amplitude;

        if (!controller.IsMoving)
        {
            // Idle Breathing
            frequency = idleFrequency;
            amplitude = idleAmplitude;
        }
        else if (controller.IsCrouching)
        {
            frequency = crouchFrequency;
            amplitude = crouchAmplitude;
        }
        else if (controller.IsSprinting)
        {
            frequency = sprintFrequency;
            amplitude = sprintAmplitude;
        }
        else
        {
            frequency = walkFrequency;
            amplitude = walkAmplitude;
        }

        // 2. Advance the timer based on speed
        timer += Time.deltaTime * frequency;

        // 3. Calculate Figure-8 Pattern (Lissajous curve)
        // Y moves up/down twice for every left/right movement (mimics footsteps)
        float xOffset = Mathf.Cos(timer / 2) * amplitude * 2f; 
        float yOffset = Mathf.Sin(timer) * amplitude;

        Vector3 targetPosition = initialPosition + new Vector3(xOffset, yOffset, 0);

        // 4. Calculate subtle Z-tilt (Sway)
        // Tilts the camera slightly in the direction of the X movement
        float zSway = Mathf.Cos(timer / 2) * amplitude * swayAmount * (controller.IsMoving ? 1f : 0f);
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, -zSway);

        // 5. Apply with Smoothing
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * positionalSmoothTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationalSmoothTime);
    }
}