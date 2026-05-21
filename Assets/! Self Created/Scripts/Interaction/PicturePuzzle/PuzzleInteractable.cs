using UnityEngine;

public class PuzzleInteractable : MonoBehaviour, IInteractable
{
    [Header("Puzzle Data")]
    [SerializeField] private PicturePuzzleSO myPuzzleData;

    public void Interact()
    {
        if (myPuzzleData != null)
        {
            PicturePuzzleManager.Instance.OpenPuzzle(myPuzzleData);
        }
        else
        {
            Debug.LogWarning("No Puzzle Data assigned");
        }
    }
}