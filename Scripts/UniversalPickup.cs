using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UniversalPickup : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private Transform originalParent;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        originalParent = transform.parent;
    }

    public void OnPickedUp(Transform holdPosition)
    {
        rb.isKinematic = true; // Turn off gravity/physics
        col.enabled = false;   // Turn off collision so it doesn't push the player

        transform.SetParent(holdPosition);
        
        // Smoothly snap to the center of the screen
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDropped()
    {
        transform.SetParent(originalParent);
        
        col.enabled = true;
        rb.isKinematic = false; 
        rb.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse); // Slight toss forward
    }
}