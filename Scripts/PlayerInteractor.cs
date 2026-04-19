using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference interactAction; 
    public InputActionReference dropAction;     

    [Header("Settings")]
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    public Transform holdPosition; 

    private GameObject currentlyHeldObject;

    private void Start()
    {
        interactAction.action.Enable();
        dropAction.action.Enable();
    }

    private void Update()
    {
        HandleInteraction();
        HandleDrop();
    }

    private void HandleInteraction()
    {
        if (interactAction.action.WasPressedThisFrame())
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                // 1. Check for Keys FIRST
                // We only check the specific object the laser touched
                KeyItem key = hit.collider.GetComponent<KeyItem>();
                if (key != null)
                {
                    key.Collect();
                    return; // Stop looking, we found a key!
                }

                // 2. Check for Universal Pickups SECOND
                UniversalPickup pickup = hit.collider.GetComponent<UniversalPickup>();
                if (pickup != null && currentlyHeldObject == null)
                {
                    PickUpObject(pickup);
                    return; // Stop looking, we picked something up!
                }

                // 3. Check for Doors/Drawers LAST
                // We use GetComponentInParent here just in case we clicked the drawer's handle
                UniversalOpenable openable = hit.collider.GetComponentInParent<UniversalOpenable>();
                if (openable != null)
                {
                    openable.Toggle();
                    return; 
                }
            }
        }
    }

    private void PickUpObject(UniversalPickup pickup)
    {
        currentlyHeldObject = pickup.gameObject;
        pickup.OnPickedUp(holdPosition);
    }

    private void HandleDrop()
    {
        if (dropAction.action.WasPressedThisFrame() && currentlyHeldObject != null)
        {
            UniversalPickup pickup = currentlyHeldObject.GetComponent<UniversalPickup>();
            pickup.OnDropped();
            currentlyHeldObject = null;
            Debug.Log("Item dropped.");
        }
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
        dropAction.action.Disable();
    }
}