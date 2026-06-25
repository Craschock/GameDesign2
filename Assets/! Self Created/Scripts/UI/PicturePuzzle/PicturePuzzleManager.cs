using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PicturePuzzleManager : MonoBehaviour
{
    public static PicturePuzzleManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject puzzleUIPanel;
    [SerializeField] private GridLayoutGroup puzzleGrid;

    [Header("Prefabs")]
    [SerializeField] private GameObject puzzlePiecePrefab;

    [Header("Input Bindings")]
    [SerializeField] private InputActionReference closeAction;

    [Header("Frame Settings")]
    [SerializeField] private RectTransform puzzleFrame;

    [SerializeField] private float frameThickness = 40f;

    private PuzzlePieceUI _firstSelectedPiece;
    private List<PuzzlePieceUI> _allPieces = new List<PuzzlePieceUI>();

    // Store current puzzle for unlocks (todo later )
    private PicturePuzzleSO _currentPuzzle;

    private float _timeOpened;

    private void Awake()
    {
        // Singlöeton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (closeAction != null)
        {
            closeAction.action.Enable();
            closeAction.action.performed += AttemptClose;
        }
    }

    private void OnDisable()
    {
        if (closeAction != null)
        {
            closeAction.action.Disable();
            closeAction.action.performed -= AttemptClose;
        }
    }

    private void Start()
    {
        puzzleUIPanel.SetActive(false);
    }

    public void OpenPuzzle(PicturePuzzleSO puzzleData)
    {
        if (puzzleUIPanel.activeSelf) return;
        _currentPuzzle = puzzleData;
        puzzleUIPanel.SetActive(true);
        _timeOpened = Time.unscaledTime;
        GeneratePuzzleBoard();
        UIManager.Instance.RegisterUIOpen();
    }

    public void ClosePuzzle()
    {
        puzzleUIPanel.SetActive(false);
        UIManager.Instance.RegisterUIClose();

        if (_firstSelectedPiece != null) _firstSelectedPiece.Highlight(false);
        _firstSelectedPiece = null;
        _currentPuzzle = null;
    }

    private void AttemptClose(InputAction.CallbackContext context)
    {
        // Only close if UI is open & tiny amount of time passed
        if (puzzleUIPanel.activeSelf && (Time.unscaledTime - _timeOpened > 0.1f))
        {
            ClosePuzzle();
        }
    }

    private void GeneratePuzzleBoard()
    {
        // Clean old puzzle
        foreach (Transform child in puzzleGrid.transform)
        {
            Destroy(child.gameObject);
        }
        _allPieces.Clear();

        // Configure grid layout
        puzzleGrid.cellSize = _currentPuzzle.cellSize;
        puzzleGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        puzzleGrid.constraintCount = _currentPuzzle.columns;

        int totalPieces = _currentPuzzle.pieces.Length;

        int columns = _currentPuzzle.columns;
        int rows = Mathf.CeilToInt((float)totalPieces / columns);

        float totalWidth = (columns * _currentPuzzle.cellSize.x) + (puzzleGrid.spacing.x * (columns - 1));
        float totalHeight = (rows * _currentPuzzle.cellSize.y) + (puzzleGrid.spacing.y * (rows - 1));

        if (puzzleFrame != null)
        {
            puzzleFrame.sizeDelta = new Vector2(totalWidth + frameThickness, totalHeight + frameThickness);
        }

        // Initialize random state
        Random.InitState(_currentPuzzle.scrambleSeed);

        // Create list of available positions
        System.Collections.Generic.List<int> availableIndices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < totalPieces; i++)
        {
            availableIndices.Add(i);
        }

        // Shuffle list
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, availableIndices.Count);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }

        // Map UI positions to correct index
        int[] instantiationOrder = new int[totalPieces];
        for (int i = 0; i < totalPieces; i++)
        {
            instantiationOrder[availableIndices[i]] = i;
        }

        for (int currentPhysicalPos = 0; currentPhysicalPos < totalPieces; currentPhysicalPos++)
        {
            // Which piece belongs in secific physical slot
            int correctDataIndex = instantiationOrder[currentPhysicalPos];
            PuzzlePieceData data = _currentPuzzle.pieces[correctDataIndex];

            // Instantiate prefab
            GameObject pieceObj = Instantiate(puzzlePiecePrefab, puzzleGrid.transform);
            PuzzlePieceUI pieceUI = pieceObj.GetComponent<PuzzlePieceUI>();

            // correctIndex = correctDataIndex, currentIndex = current physical position
            pieceUI.Initialize(correctPos: correctDataIndex, currentPos: currentPhysicalPos, pieceSprite: data.pieceSprite);
            _allPieces.Add(pieceUI);
        }
    }

    public void OnPieceSelected(PuzzlePieceUI clickedPiece)
    {
        if (_firstSelectedPiece == null)
        {
            // First click
            _firstSelectedPiece = clickedPiece;
            _firstSelectedPiece.Highlight(true);
        }
        else
        {
            // Second click to swap
            SwapPieces(_firstSelectedPiece, clickedPiece);

            _firstSelectedPiece.Highlight(false);
            _firstSelectedPiece = null;

            CheckWinCondition();
        }
    }

    private void SwapPieces(PuzzlePieceUI pieceA, PuzzlePieceUI pieceB)
    {
        // Swap index
        int tempIndex = pieceA.currentIndex;
        pieceA.UpdateCurrentIndex(pieceB.currentIndex);
        pieceB.UpdateCurrentIndex(tempIndex);

        // Swap physical position
        int siblingIndexA = pieceA.transform.GetSiblingIndex();
        int siblingIndexB = pieceB.transform.GetSiblingIndex();

        pieceA.transform.SetSiblingIndex(siblingIndexB);
        pieceB.transform.SetSiblingIndex(siblingIndexA);
    }

    private void CheckWinCondition()
    {
        foreach (var piece in _allPieces)
        {
            // If one piece is wrong, continue
            if (piece.currentIndex != piece.correctIndex) return;
        }

        Debug.Log("Bro hat das puzzle hin bekommen :)");

    /*
    *   Disable Interactable
    *   Add Memory to book
    *   und halt den rest ig
    */

    //    ClosePuzzle();
    }
}