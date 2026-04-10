using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 TouchDist;

    private Vector2 pointerOld;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerOld = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pointerNew = eventData.position;
        TouchDist = pointerNew - pointerOld;
        pointerOld = pointerNew;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TouchDist = Vector2.zero;
    }
}