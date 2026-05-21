using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PuzzlePieceUI : MonoBehaviour
{
    [Header("Puzzle Indices")]
    public int correctIndex;
    public int currentIndex;

    private Image _image;
    private Button _button;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();

        // Signal manager when piece is clicked
        _button.onClick.AddListener(() => PicturePuzzleManager.Instance.OnPieceSelected(this));
    }

    public void Initialize(int correctPos, int currentPos, Sprite pieceSprite)
    {
        correctIndex = correctPos;
        currentIndex = currentPos;
        _image.sprite = pieceSprite;
    }

    public void UpdateCurrentIndex(int newIndex)
    {
        currentIndex = newIndex;
    }

    public void Highlight(bool isSelected)
    {
        // Visual feedback for when piece clicked
        _image.color = isSelected ? Color.gray : Color.white;
    }
}