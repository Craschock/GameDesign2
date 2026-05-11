using UnityEngine;

public class MemoryPickup : MonoBehaviour, IInteractable
{
    [Header("What does this unlock?")]
    [SerializeField] private MemoryFragmentSO fragmentToUnlock;

    public void Interact()
    {
        if (fragmentToUnlock != null && !fragmentToUnlock.isUnlocked)
        {
            Debug.Log($"Unlocked memory: {fragmentToUnlock.title}");

            // Unlock data
            fragmentToUnlock.isUnlocked = true;

            // If book is open, refresh it
            DementiaBookManager.Instance.UpdatePageDisplay();

            // Disable pickup
            gameObject.SetActive(false);
        }
    }
}