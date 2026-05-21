using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DementiaBookManager : MonoBehaviour
{
    public static DementiaBookManager Instance { get; private set; }

    [Header("Book Data")]
    [SerializeField] private List<MemoryFragmentSO> allPages;


    [Header("UI References")]
    [SerializeField] private GameObject bookUIPanel;
    [SerializeField] private InputActionReference toggleBookAction;

    [Header("UI - Left Page")]
    [SerializeField] private TextMeshProUGUI leftTitleText;
    [SerializeField] private TextMeshProUGUI leftContentText;
    [SerializeField] private Image leftMemoryImage;

    [Header("UI - Right Page")]
    [SerializeField] private TextMeshProUGUI rightTitleText;
    [SerializeField] private TextMeshProUGUI rightContentText;
    [SerializeField] private Image rightMemoryImage;

    [Header("String Text")]
    [Tooltip("Text für titel, der versteckt ist")]
    [SerializeField] public string hiddenTitle = "???";
    [Tooltip("Text für inhalt, der versteckt ist")]
    [SerializeField] public string hiddenContent = "I can't remember this...";

    private int _currentPageIndex = 0;
    private bool _isBookOpen = false;

    // UI Event
    public static event System.Action<bool> OnUIStateChanged;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (toggleBookAction != null)
        {
            toggleBookAction.action.Enable();
            toggleBookAction.action.performed += ToggleBook;
        }
    }

    private void OnDisable()
    {
        if (toggleBookAction != null)
        {
            toggleBookAction.action.Disable();
            toggleBookAction.action.performed -= ToggleBook;
        }
    }

    private void Start()
    {
        bookUIPanel.SetActive(false);
    }

    private void ToggleBook(InputAction.CallbackContext context)
    {
        _isBookOpen = !_isBookOpen;
        OnUIStateChanged?.Invoke(_isBookOpen);
        bookUIPanel.SetActive(_isBookOpen);

        if (_isBookOpen)
        {
            // Pause game and unlock cursor
            UIManager.Instance.RegisterUIOpen();
            UpdatePageDisplay();
        }
        else
        {
            // Resume game and lock cursor
            UIManager.Instance.RegisterUIClose();
        }
    }

    // Call from UI Buttons to flip pages
    public void ChangePage(int direction)
    {
        _currentPageIndex += (direction * 2);

        // Calculate maximum index for LEFT page
        int maxLeftIndex = allPages.Count - 1;
        if (maxLeftIndex % 2 != 0) maxLeftIndex -= 1;

        // Clamp index
        _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, Mathf.Max(0, maxLeftIndex));
      
        UpdatePageDisplay();
    }

    public void UpdatePageDisplay()
    {
        if (allPages.Count == 0) return;

        // Set Left Page
        UpdateSinglePage(leftTitleText, leftContentText, leftMemoryImage, _currentPageIndex);

        // Set Right Page
        if (_currentPageIndex + 1 < allPages.Count)
        {
            UpdateSinglePage(rightTitleText, rightContentText, rightMemoryImage, _currentPageIndex + 1);
        }
        else
        {
            ClearPageUI(rightTitleText, rightContentText, rightMemoryImage);
        }
    }


    // Just to actually "draw" page visiblr
    private void UpdateSinglePage(TextMeshProUGUI titleUI, TextMeshProUGUI contentUI, Image imageUI, int index)
    {
        MemoryFragmentSO page = allPages[index];

        if (page.isUnlocked)
        {
            titleUI.text = page.title;
            contentUI.text = page.content;
            imageUI.sprite = page.photo;
            imageUI.color = Color.white;
        }
        else
        {
            titleUI.text = hiddenTitle;
            contentUI.text = hiddenContent;
            imageUI.sprite = null;
            imageUI.color = new Color(0, 0, 0, 0);
        }
    }

    // Just to clear page
    private void ClearPageUI(TextMeshProUGUI titleUI, TextMeshProUGUI contentUI, Image imageUI)
    {
        titleUI.text = "";
        contentUI.text = "";
        imageUI.sprite = null;
        imageUI.color = new Color(0, 0, 0, 0);
    }
}