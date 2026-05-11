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
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Image memoryImage;

    [Header("Input")]
    [SerializeField] private InputActionReference toggleBookAction;

    [Header("String Text")]
    [SerializeField] public string title = "???";
    [SerializeField] public string content = "I can't remember this...";

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
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpdatePageDisplay();
        }
        else
        {
            // Resume game and lock cursor
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Call from UI Buttons to flip pages
    public void ChangePage(int direction)
    {
        _currentPageIndex += direction;

        // Clamp index
        _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, allPages.Count - 1);
        UpdatePageDisplay();
    }

    public void UpdatePageDisplay()
    {
        if (allPages.Count == 0) return;

        MemoryFragmentSO currentPage = allPages[_currentPageIndex];

        if (currentPage.isUnlocked)     // Display Page Data when Unlocked
        {
            titleText.text = currentPage.title;
            contentText.text = currentPage.content;
            memoryImage.sprite = currentPage.photo;
            memoryImage.color = Color.white;
        }
        else                            // Display nothing when not Unlocked
        {
            titleText.text = title;
            contentText.text = content;
            memoryImage.sprite = null;
            memoryImage.color = new Color(0, 0, 0, 0);
        }
    }
}