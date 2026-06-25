using UnityEngine;
using System.Collections.Generic;

public enum GuitarLane
{
    Lane_1 = 0,
    Lane_2 = 1,
    Lane_3 = 2,
    Lane_4 = 3
}

[CreateAssetMenu(fileName = "New Guitar Song", menuName = "Dementia/Guitar Song")]
public class GuitarSongSO : ScriptableObject
{
    [Header("Song Settings")]
    public float noteSpeed = 500f;
    public float hitTolerance = 0.15f;

    [Header("Notes")]
    public GuitarNoteData[] notes;
}

[System.Serializable]
public struct GuitarNoteData
{
    [Tooltip("Select the lane from the dropdown.")]
    public GuitarLane lane;

    [Tooltip("Drag to set the hit time in seconds.")]
    [Range(0f, 300f)]
    public float hitTime;
}