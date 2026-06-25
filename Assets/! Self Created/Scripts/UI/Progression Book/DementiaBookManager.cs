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

    [Header("Page Containers")]
    [Tooltip("Parent object for the custom first pages (About Me).")]
    [SerializeField] private GameObject aboutMeContainer;
    [Tooltip("Parent object for the standard memory layout.")]
    [SerializeField] private GameObject memoryContainer;

    [Header("Book Settings")]
    [Tooltip("How many pages are reserved at the start before memories begin. (Must be an even number)")]
    [SerializeField] private int reservedStartPages = 2;

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
    [Tooltip("Text für Titel, der versteckt ist")]
    [SerializeField] public string hiddenTitle = "???";

    [Tooltip("Text für Inhalt, der versteckt ist")]
    [SerializeField] public string hiddenContent = "I can't remember this...";

    private int _currentPageIndex = 0;
    private bool _isBookOpen = false;

    // UI Event
    public static event System.Action<bool> OnUIStateChanged;

    private void Awake()
    {
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

        if (!_isBookOpen && UIManager.Instance.IsAnyUIOpen) return;

        _isBookOpen = !_isBookOpen;
        OnUIStateChanged?.Invoke(_isBookOpen);
        bookUIPanel.SetActive(_isBookOpen);

        if (_isBookOpen)
        {
            UIManager.Instance.RegisterUIOpen();
            UpdatePageDisplay();
        }
        else
        {
            UIManager.Instance.RegisterUIClose();
        }
    }

    public void ChangePage(int direction)
    {
        _currentPageIndex += (direction * 2);

        int maxLeftIndex = allPages.Count - 1 + reservedStartPages;
        if (maxLeftIndex % 2 != 0) maxLeftIndex -= 1;

        _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, Mathf.Max(0, maxLeftIndex));

        UpdatePageDisplay();
    }

    public void UpdatePageDisplay()
    {
        if (_currentPageIndex < reservedStartPages)
        {
            if (aboutMeContainer != null) aboutMeContainer.SetActive(true);
            if (memoryContainer != null) memoryContainer.SetActive(false);
        }
        else
        {
            if (aboutMeContainer != null) aboutMeContainer.SetActive(false);
            if (memoryContainer != null) memoryContainer.SetActive(true);

            if (allPages.Count == 0) return;

            int memoryIndex = _currentPageIndex - reservedStartPages;

            if (memoryIndex < allPages.Count)
                UpdateSinglePage(leftTitleText, leftContentText, leftMemoryImage, memoryIndex);
            else
                ClearPageUI(leftTitleText, leftContentText, leftMemoryImage);

            if (memoryIndex + 1 < allPages.Count)
                UpdateSinglePage(rightTitleText, rightContentText, rightMemoryImage, memoryIndex + 1);
            else
                ClearPageUI(rightTitleText, rightContentText, rightMemoryImage);
        }
    }

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

    private void ClearPageUI(TextMeshProUGUI titleUI, TextMeshProUGUI contentUI, Image imageUI)
    {
        titleUI.text = "";
        contentUI.text = "";
        imageUI.sprite = null;
        imageUI.color = new Color(0, 0, 0, 0);
    }
}