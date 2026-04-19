using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorComponent : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Transform playerCamera;

    [Header("Input Bindings")]
    [SerializeField] private InputActionReference interactAction;

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            // Subscribe to the button press event
            interactAction.action.performed += AttemptInteraction;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.Disable();
            // Unsubscribe
            interactAction.action.performed -= AttemptInteraction;
        }
    }

    private void AttemptInteraction(InputAction.CallbackContext context)
    {
        if (playerCamera == null) return;

        // Shoot ray from  camera
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            // Check if hit object is interactable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}