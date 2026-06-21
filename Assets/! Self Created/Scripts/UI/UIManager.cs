using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public static event System.Action<bool> OnAnyUIStateChanged;

    private int _openUIWindows = 0;

    public bool IsAnyUIOpen => _openUIWindows > 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterUIOpen()
    {
        _openUIWindows++;

        // If menu opens, tell game to pause/unlock cursor
        if (_openUIWindows == 1)
        {
            // Pause Game
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnAnyUIStateChanged?.Invoke(true);
        }
    }

    public void RegisterUIClose()
    {
        _openUIWindows--;

        // If ALL menus closed, tell game to resume/lock cursor
        if (_openUIWindows <= 0)
        {
            _openUIWindows = 0;

            // Resume Game
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnAnyUIStateChanged?.Invoke(false);
        }
    }
}