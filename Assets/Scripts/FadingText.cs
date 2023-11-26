using System.Collections;
using TMPro;
using UnityEngine;

public class FadingText : MonoBehaviour
{
    [SerializeField] private float halfFadeTime = 0.5f;

    private TextMeshProUGUI _textComponent;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Coroutine FadeTextRoutine() => StartCoroutine(FadeText());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coroutine"></param>
    public void StopFadeTextCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeText()
    {
        while (true)
        {
            var color = _textComponent.color;
            var startAlpha = color.a;

            var elapsedTime = 0f;
            while (elapsedTime < halfFadeTime)
            {
                elapsedTime += Time.deltaTime;
                var newColor = new Color(color.r, color.g, color.b,
                    Mathf.Lerp(startAlpha, 0.5f, elapsedTime / halfFadeTime));
                _textComponent.color = newColor;
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < halfFadeTime)
            {
                elapsedTime += Time.deltaTime;
                var newColor = new Color(color.r, color.g, color.b,
                    Mathf.Lerp(0.5f, startAlpha, elapsedTime / halfFadeTime));
                _textComponent.color = newColor;
                yield return null;
            }

            yield return null;
        }
    }
}