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
        player.SetLookInput(touchField.TouchDist);
    }
}