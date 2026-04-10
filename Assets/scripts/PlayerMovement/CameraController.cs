using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;
    private int lookFingerId = -1;

    void Update()
    {
        if (Input.touchCount == 0)
        {
            lookFingerId = -1;
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            // ❗ Ignore touches on UI (like joystick)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                continue;

            // Detect right-side touch
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x > Screen.width / 2f)
                {
                    lookFingerId = touch.fingerId;
                }
            }

            if (touch.fingerId == lookFingerId)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    float touchX = touch.deltaPosition.x * sensitivity;
                    float touchY = touch.deltaPosition.y * sensitivity;

                    xRotation -= touchY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    playerBody.Rotate(Vector3.up * touchX);
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    lookFingerId = -1;
                }
            }
        }
    }
}
