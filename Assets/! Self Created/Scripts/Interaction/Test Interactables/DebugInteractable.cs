using UnityEngine;

public class DebugInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string debugMessage = "Interaction Initiated";

    // Implementing interface method
    public void Interact()
    {
        Debug.Log(debugMessage);
    }
}