using System.Collections;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    [SerializeField] private bool isPlayerPiece = true;

    [SerializeField] private SpriteRenderer tileHighlight;
    [SerializeField] private SpriteRenderer progressBar;

    [SerializeField] private Animator progressBarAnimator;

    private float _timeToHold;
    private const int NumSteps = 5;
    
    private static readonly int ProgressNumber = Animator.StringToHash("ProgressNumber");
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="highlight"></param>
    public void SetHighlight(bool highlight)
    {
        tileHighlight.enabled = highlight;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerPiece() => isPlayerPiece;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeToHold"></param>
    /// <returns></returns>
    public Coroutine StartProgressBar(float timeToHold)
    {
        _timeToHold = timeToHold;
        progressBar.enabled = true;
        return StartCoroutine(ProgressBar(0));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="progressNumber"></param>
    /// <returns></returns>
    private IEnumerator ProgressBar(int progressNumber)
    {
        progressBarAnimator.SetInteger(ProgressNumber, progressNumber);
        
        yield return new WaitForSeconds(_timeToHold / NumSteps);
        
        progressNumber++;
        if (progressNumber < NumSteps)
        {
            yield return ProgressBar(progressNumber);
        }
        else
        {
            progressBarAnimator.SetInteger(ProgressNumber, 0);
            progressBar.enabled = false;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="coroutine"></param>
    public void StopProgressBar(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        progressBarAnimator.SetInteger(ProgressNumber, 0);
        progressBar.enabled = false;
    }
}