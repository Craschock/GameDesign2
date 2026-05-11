using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorComponent : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Transform playerCamera;

    [Header("Layer Settings")]
    [SerializeField] private string interactableLayerName = "Interactable";
    [SerializeField] private string selectedLayerName = "InteractableSelected";

    [Header("Input Bindings")]
    [SerializeField] private InputActionReference interactAction;

    // Cached Layers for Outline
    private int _interactableLayer;
    private int _selectedLayer;

    // State Tracking
    private GameObject _currentHoveredObject;
    private IInteractable _currentInteractable;

    private void Start()
    {
        // Convert Layer Name to integer IDs
        _interactableLayer = LayerMask.NameToLayer(interactableLayerName);
        _selectedLayer = LayerMask.NameToLayer(selectedLayerName);
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += AttemptInteraction;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.Disable();
            interactAction.action.performed -= AttemptInteraction;
        }
    }

    private void Update()
    {
        HandleHover();
    }

    // Function for handling Raycast
    private void HandleHover()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Looking at a new object?
            if (hitObject != _currentHoveredObject)
            {
                ClearHover(); // Reset old object first

                // Check if new object has IInteractable interface
                IInteractable interactable = hitObject.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _currentHoveredObject = hitObject;
                    _currentInteractable = interactable;

                    // Apply outline layer
                    _currentHoveredObject.layer = _selectedLayer;
                }
            }
        }
        else
        {
            // The raycast hit nothing
            ClearHover();
        }
    }

    private void ClearHover()
    {
        // If we were looking at something, reset it
        if (_currentHoveredObject != null)
        {
            // 🛑 Revert to original layer
            _currentHoveredObject.layer = _interactableLayer;

            _currentHoveredObject = null;
            _currentInteractable = null;
        }
    }

    private void AttemptInteraction(InputAction.CallbackContext context)
    {
        // Interact with cached object
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
    }
}