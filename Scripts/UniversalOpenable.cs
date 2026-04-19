using UnityEngine;
using System.Collections;

public class UniversalOpenable : MonoBehaviour
{
    public enum OpenType { Slide, Rotate }
    
    [Header("Settings")]
    public OpenType openType = OpenType.Slide;
    public float openSpeed = 5f;
    
    [Header("Slide Settings (For Drawers)")]
    public Vector3 closedPosition;
    public Vector3 openPosition;

    [Header("Rotate Settings (For Doors)")]
    public Vector3 closedRotation;
    public Vector3 openRotation;

    private bool isOpen = false;
    private Coroutine moveCoroutine;

    [ContextMenu("Set Closed State to Current")]
    public void SetClosedState() // Right-click component in inspector to auto-set
    {
        closedPosition = transform.localPosition;
        closedRotation = transform.localEulerAngles;
    }

    [ContextMenu("Set Open State to Current")]
    public void SetOpenState() // Right-click component in inspector to auto-set
    {
        openPosition = transform.localPosition;
        openRotation = transform.localEulerAngles;
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        
        if (openType == OpenType.Slide)
            moveCoroutine = StartCoroutine(SlideRoutine(isOpen ? openPosition : closedPosition));
        else
            moveCoroutine = StartCoroutine(RotateRoutine(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator SlideRoutine(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localPosition = targetPos;
    }

    private IEnumerator RotateRoutine(Vector3 targetRot)
    {
        Quaternion target = Quaternion.Euler(targetRot);
        while (Quaternion.Angle(transform.localRotation, target) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localRotation = target;
    }
}