using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float durationMinimum;
    [SerializeField] private float durationMaximum;
    [SerializeField] private float magnitudeMinimum;
    [SerializeField] private float magnitudeMaximum;
    
    private float _duration;
    private float _magnitude;
    
    private bool _shakeStarted = false;
    
    /// <summary>
    /// Subscribes to the OnCameraShake event.
    /// </summary>
    private void Start()
    {
        GameEvent.OnCameraShake += StartShake;
    }

    /// <summary>
    /// Starts the camera shake coroutine with random duration and magnitude, if it has not already started.
    /// </summary>
    private void StartShake()
    {
        if (_shakeStarted) return;

        _duration = Random.Range(durationMinimum, durationMaximum);
        _magnitude = Random.Range(magnitudeMinimum, magnitudeMaximum);
        _shakeStarted = true;
        StartCoroutine(Shake());
    }
    
    /// <summary>
    /// Shakes the camera for a random duration and magnitude.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shake()
    {
        var originalPos = transform.localPosition;

        var elapsed = 0f;

        while (elapsed < _duration)
        {
            var x = Random.Range(-1f, 1f) * _magnitude;
            var y = Random.Range(-1f, 1f) * _magnitude;
            
            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            
            yield return null;
        }
        
        transform.localPosition = originalPos;
        _shakeStarted = false;
    }
    
    /// <summary>
    /// Unsubscribes from the OnCameraShake event.
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnCameraShake -= StartShake;
    }
}