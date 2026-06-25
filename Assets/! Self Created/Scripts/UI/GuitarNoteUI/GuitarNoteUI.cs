using UnityEngine;

public class GuitarNoteUI : MonoBehaviour
{
    private float _hitTime;
    private float _spawnTime;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private int _lane;

    public float HitTime => _hitTime;
    public int Lane => _lane;

    public void Initialize(int lane, float hitTime, float spawnTime, Vector3 startPos, Vector3 endPos)
    {
        _lane = lane;
        _hitTime = hitTime;
        _spawnTime = spawnTime;
        _startPos = startPos;
        _endPos = endPos;

        transform.position = _startPos;
    }

    public void UpdatePosition(float currentTime)
    {
        float progress = (currentTime - _spawnTime) / (_hitTime - _spawnTime);

        transform.position = Vector3.LerpUnclamped(_startPos, _endPos, progress);
    }
}