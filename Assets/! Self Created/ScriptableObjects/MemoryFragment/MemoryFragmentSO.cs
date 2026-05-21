using UnityEngine;

[CreateAssetMenu(fileName = "New Memory", menuName = "Dementia/Memory Fragment")]
public class MemoryFragmentSO : ScriptableObject
{
    public string memoryId; // later for save systems
    public string title = "Memory Name";

    [TextArea(3, 10)]
    public string content;
    public Sprite photo;
 
    // Default is false
    // false = hidden, true = visible
    public bool isUnlocked = false;
}