using UnityEngine;

public class GuitarInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GuitarSongSO songData;

    public void Interact()
    {
        if (songData != null)
        {
            GuitarMiniGameManager.Instance.OpenMiniGame(songData);
        }
    }
}