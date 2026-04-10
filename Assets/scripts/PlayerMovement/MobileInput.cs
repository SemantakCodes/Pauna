using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public JoyStick joyStick;
    public FixedTouchField touchField;

    private PlayerController player;

    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Movement input
        player.SetMoveInput(joyStick.InputDirection);

        // Camera input
        player.SetLookInput(Vector2.Lerp(
        player.lookInput,
        touchField.TouchDist * 0.2f,
        Time.deltaTime * 10f));
    }
}