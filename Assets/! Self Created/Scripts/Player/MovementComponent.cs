using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input Bindings")]
    [SerializeField] private InputActionReference moveAction;

    private CharacterController _controller;

    private void Awake()
    {
        // Grab the required controller component (see line 4)
        _controller = GetComponent<CharacterController>();
    }

    private void OnEnable() // Enable Input
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable() // Disable Input
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update() 
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveAction == null) return;

        // Read WASD Vector2 value
        Vector2 inputDir = moveAction.action.ReadValue<Vector2>();

        // Calculate movement direction relative to player local rotation
        Vector3 move = transform.right * inputDir.x + transform.forward * inputDir.y;

        // Apply
        _controller.Move(move * (moveSpeed * Time.deltaTime));
    }
}