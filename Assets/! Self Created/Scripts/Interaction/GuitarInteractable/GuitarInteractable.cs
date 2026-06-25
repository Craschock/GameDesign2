using System.Collections;
using UnityEngine;

public class GuitarInteractable : MonoBehaviour, IInteractable
{
    [Header("Minigame Data")]
    [SerializeField] private GuitarSongSO songData;

    [Header("Animation References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform guitarTargetSlot;
    [SerializeField] private Transform lookAtTarget;

    [Header("Animation Settings")]
    [SerializeField] private float stepDuration = 1.2f;
    [SerializeField] private Vector3 finalGuitarTilt = new Vector3(-45f, 0f, 90f);
    [SerializeField] private Vector3 finalCameraLookDown = new Vector3(45f, 0f, 0f);

    private bool _isAnimating = false;

    public void Interact()
    {
        if (songData != null && !_isAnimating)
        {
            StartCoroutine(PlayInteractionSequence());
        }
    }

    private IEnumerator PlayInteractionSequence()
    {
        _isAnimating = true;

        var movement = playerTransform.GetComponent<MovementComponent>();
        var look = playerTransform.GetComponentInChildren<LookComponent>();
        if (movement != null) movement.enabled = false;
        if (look != null) look.enabled = false;

        yield return StartCoroutine(SmoothTransition(
            transform,
            transform.position, guitarTargetSlot.position,
            transform.rotation, guitarTargetSlot.rotation
        ));

        transform.SetParent(guitarTargetSlot);

        Vector3 lookDir = lookAtTarget.position - playerTransform.position;
        lookDir.y = 0;
        Quaternion targetPlayerRot = Quaternion.LookRotation(lookDir);

        Quaternion targetCamUpRot = Quaternion.LookRotation(lookAtTarget.position - playerCamera.position);

        StartCoroutine(SmoothTransition(
            playerCamera,
            playerCamera.position, playerCamera.position,
            playerCamera.rotation, targetCamUpRot
        ));

        yield return StartCoroutine(SmoothTransition(
            playerTransform,
            playerTransform.position, playerTransform.position,
            playerTransform.rotation, targetPlayerRot
        ));

        yield return new WaitForSeconds(0.4f);

        Quaternion startCamRot = playerCamera.localRotation;
        Quaternion targetCamRotLocal = Quaternion.Euler(finalCameraLookDown);

        yield return StartCoroutine(SmoothTransition(
            playerCamera,
            playerCamera.position, playerCamera.position,
            startCamRot, targetCamRotLocal, true
        ));

        Quaternion startGuitarRot = transform.localRotation;
        Quaternion targetGuitarRot = Quaternion.Euler(finalGuitarTilt);
        yield return StartCoroutine(SmoothTransition(
            transform,
            transform.position, transform.position,
            startGuitarRot, targetGuitarRot, true
        ));

        if (movement != null) movement.enabled = true;
        if (look != null) look.enabled = true;

        GuitarMiniGameManager.Instance.OpenMiniGame(songData);
        _isAnimating = false;
    }

    private IEnumerator SmoothTransition(Transform target, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, bool isLocalRotation = false)
    {
        float elapsed = 0f;
        while (elapsed < stepDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / stepDuration);

            target.position = Vector3.Lerp(startPos, endPos, t);

            if (isLocalRotation)
                target.localRotation = Quaternion.Lerp(startRot, endRot, t);
            else
                target.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        target.position = endPos;
        if (isLocalRotation) target.localRotation = endRot;
        else target.rotation = endRot;
    }
}