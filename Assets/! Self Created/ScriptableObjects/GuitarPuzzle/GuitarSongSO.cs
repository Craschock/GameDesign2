using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Guitar Song", menuName = "Dementia/Guitar Song")]
public class GuitarSongSO : ScriptableObject
{
    public float noteSpeed = 500f;
    public float hitTolerance = 0.15f;
    public GuitarNoteData[] notes;
}

[System.Serializable]
public struct GuitarNoteData
{
    public int laneIndex;
    public float hitTime;
}