using UnityEngine;

public class GuitarNoteUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    private float _hitTime;
    private float _spawnY;
    private float _hitY;
    private float _speed;
    private int _lane;

    public float HitTime => _hitTime;
    public int Lane => _lane;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(int lane, float hitTime, float spawnY, float hitY, float speed)
    {
        _lane = lane;
        _hitTime = hitTime;
        _spawnY = spawnY;
        _hitY = hitY;
        _speed = speed;

        Vector2 pos = _rectTransform.anchoredPosition;
        pos.y = _spawnY;
        _rectTransform.anchoredPosition = pos;
    }

    public void UpdatePosition(float currentTime)
    {
        float timeDifference = _hitTime - currentTime;
        float distance = timeDifference * _speed;

        Vector2 pos = _rectTransform.anchoredPosition;
        pos.y = _hitY + distance;
        _rectTransform.anchoredPosition = pos;
    }
}