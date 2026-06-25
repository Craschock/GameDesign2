using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GuitarMiniGameManager : MonoBehaviour
{
    public static GuitarMiniGameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject guitarUIPanel;
    [SerializeField] private RectTransform spawnLine;
    [SerializeField] private RectTransform[] hitZones;
    [SerializeField] private Transform notesContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject notePrefab;

    [Header("Input Bindings")]
    [SerializeField] private InputActionReference[] laneActions = new InputActionReference[4];
    [SerializeField] private InputActionReference closeAction;

    private GuitarSongSO _currentSong;
    private List<GuitarNoteUI> _activeNotes = new List<GuitarNoteUI>();

    private float _songTimer = 0f;
    private float _spawnLeadTime = 0f;
    private int _nextNoteIndex = 0;
    private bool _isPlaying = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        foreach (var action in laneActions)
        {
            if (action != null)
            {
                action.action.Enable();
                action.action.performed += HandleLaneInput;
            }
        }
        if (closeAction != null)
        {
            closeAction.action.Enable();
            closeAction.action.performed += AttemptClose;
        }
    }

    private void OnDisable()
    {
        foreach (var action in laneActions)
        {
            if (action != null)
            {
                action.action.Disable();
                action.action.performed -= HandleLaneInput;
            }
        }
        if (closeAction != null)
        {
            closeAction.action.Disable();
            closeAction.action.performed -= AttemptClose;
        }
    }

    private void Start()
    {
        guitarUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (!_isPlaying || _currentSong == null) return;

        _songTimer += Time.unscaledDeltaTime;

        while (_nextNoteIndex < _currentSong.notes.Length)
        {
            GuitarNoteData nextNoteData = _currentSong.notes[_nextNoteIndex];
            if (nextNoteData.hitTime - _songTimer <= _spawnLeadTime)
            {
                SpawnNote(nextNoteData);
                _nextNoteIndex++;
            }
            else
            {
                break;
            }
        }

        for (int i = _activeNotes.Count - 1; i >= 0; i--)
        {
            GuitarNoteUI note = _activeNotes[i];
            note.UpdatePosition(_songTimer);

            if (_songTimer - note.HitTime > 1.5f)
            {
                Destroy(note.gameObject);
                _activeNotes.RemoveAt(i);
            }
        }

        if (_nextNoteIndex >= _currentSong.notes.Length && _activeNotes.Count == 0)
        {
            CloseMiniGame();
        }
    }

    public void OpenMiniGame(GuitarSongSO song)
    {
        if (UIManager.Instance.IsAnyUIOpen) return;

        _currentSong = song;
        _songTimer = 0f;
        _nextNoteIndex = 0;
        _isPlaying = true;

        float physicalDistance = Vector3.Distance(spawnLine.position, hitZones[0].position);
        _spawnLeadTime = physicalDistance / _currentSong.noteSpeed;

        guitarUIPanel.SetActive(true);
        UIManager.Instance.RegisterUIOpen();
    }

    public void CloseMiniGame()
    {
        _isPlaying = false;
        _currentSong = null;

        foreach (var note in _activeNotes)
        {
            if (note != null) Destroy(note.gameObject);
        }
        _activeNotes.Clear();

        guitarUIPanel.SetActive(false);
        UIManager.Instance.RegisterUIClose();
    }

    private void AttemptClose(InputAction.CallbackContext context)
    {
        if (guitarUIPanel.activeSelf)
        {
            CloseMiniGame();
        }
    }

    private void SpawnNote(GuitarNoteData data)
    {
        GameObject noteObj = Instantiate(notePrefab, notesContainer);
        GuitarNoteUI noteUI = noteObj.GetComponent<GuitarNoteUI>();

        Transform targetHitZone = hitZones[(int)data.lane];

        Vector3 startWorldPos = new Vector3(targetHitZone.position.x, spawnLine.position.y, targetHitZone.position.z);
        Vector3 endWorldPos = targetHitZone.position;

        noteUI.Initialize((int)data.lane, data.hitTime, _songTimer, startWorldPos, endWorldPos);

        _activeNotes.Add(noteUI);
    }

    private void HandleLaneInput(InputAction.CallbackContext context)
    {
        if (!_isPlaying) return;

        int laneIndex = -1;
        for (int i = 0; i < laneActions.Length; i++)
        {
            if (context.action == laneActions[i].action)
            {
                laneIndex = i;
                break;
            }
        }

        if (laneIndex == -1) return;

        GuitarNoteUI closestNote = null;
        float closestTime = float.MaxValue;
        int noteToRemove = -1;

        for (int i = 0; i < _activeNotes.Count; i++)
        {
            GuitarNoteUI note = _activeNotes[i];
            if (note.Lane == laneIndex)
            {
                float timeDiff = Mathf.Abs(note.HitTime - _songTimer);
                if (timeDiff < _currentSong.hitTolerance && timeDiff < closestTime)
                {
                    closestTime = timeDiff;
                    closestNote = note;
                    noteToRemove = i;
                }
            }
        }

        if (closestNote != null)
        {
            Destroy(closestNote.gameObject);
            _activeNotes.RemoveAt(noteToRemove);
        }
    }
}