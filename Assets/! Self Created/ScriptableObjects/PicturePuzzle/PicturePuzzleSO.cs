using UnityEngine;

[CreateAssetMenu(fileName = "New Picture Puzzle", menuName = "Dementia/Picture Puzzle")]
public class PicturePuzzleSO : ScriptableObject
{
    [Header("Randomization Settings")]
    [Tooltip("Fixed seed for testing.")]
    public int scrambleSeed = 42;

    [Header("Grid Settings")]
    public Vector2 cellSize = new Vector2(200, 200);
    public int columns = 3;

    [Header("Puzzle Data")]
    [Tooltip("Add pieces in correct order. 0 is top-left. Left to right. From 0 to X")]
    public PuzzlePieceData[] pieces;
}

[System.Serializable]
public struct PuzzlePieceData
{
    public Sprite pieceSprite;
}