using UnityEngine;
using UnityEngine.InputSystem;

public class LookComponent : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 15f;
    [SerializeField] private float verticalLookLimit = 85f; // Max look up angle

    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("Input Bindings")]
    [SerializeField] private InputActionReference lookAction;

    private float _xRotation = 0f;

    private void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()  // Enable Input
    {
        if (lookAction != null) lookAction.action.Enable();
    }

    private void OnDisable()  // Disable Input
    {
        if (lookAction != null) lookAction.action.Disable();
    }

    private void Update()
    {
        HandleLook();
    }

    private void HandleLook()
    {
        if (lookAction == null || playerCamera == null) return;

        // Read the delta movement from the mouse
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float lookX = lookInput.x * lookSensitivity * 0.01f;
        float lookY = lookInput.y * lookSensitivity * 0.01f;

        // Vertical look
        _xRotation -= lookY;
        _xRotation = Mathf.Clamp(_xRotation, -verticalLookLimit, verticalLookLimit);
        playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Horizontal look
        transform.Rotate(Vector3.up * lookX);
    }
}