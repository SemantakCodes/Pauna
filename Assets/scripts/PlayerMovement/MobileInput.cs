using UnityEngine;


public class MobileInput : MonoBehaviour
{
    public JoyStick joyStick;
    public FixedTouchField TouchField;

    

    // Update is called once per frame
    void Update()
    {
        var fps = GetComponent<PlayerController>();
        fps.RunAxis = joyStick.InputDirection;
        fps.LookAxis = TouchField.TouchDist;

    }
}
