using UnityEngine;

public class HeadBobComponent : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float bobFrequency = 14f;
    [SerializeField] private float bobHorizontalAmplitude = 0.05f;
    [SerializeField] private float bobVerticalAmplitude = 0.05f;
    [SerializeField] private float headBobSmoothing = 10f;

    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private CharacterController characterController;

    private Vector3 _startPos;
    private float _timer;

    private void Start()
    {
        if (playerCamera != null)
        {
            // Store the initial local position of camera
            _startPos = playerCamera.localPosition;
        }
    }

    private void Update()
    {
        HandleHeadBob();
    }

    private void HandleHeadBob()
    {
        if (playerCamera == null || characterController == null) return;

        // Calculate horizontal speed
        Vector3 flatVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        float speed = flatVelocity.magnitude;

        Vector3 targetPos = _startPos;

        // Moving faster than threshold, apply bob
        if (speed > 0.1f)
        {
            _timer += Time.deltaTime * (bobFrequency * (speed * 0.2f));
            targetPos.y += Mathf.Sin(_timer) * bobVerticalAmplitude;
            targetPos.x += Mathf.Cos(_timer / 2f) * bobHorizontalAmplitude;
        }
        else
        {
            // Reset timer
            _timer = 0f;
        }

        // Interpolate camera to target position
        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetPos, Time.deltaTime * headBobSmoothing);
    }
}