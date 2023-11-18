using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    [SerializeField] private bool isPlayerPiece = true;

    [SerializeField] private SpriteRenderer tileHighlight;
    [SerializeField] private SpriteRenderer progressBar;

    [SerializeField] private Animator progressBarAnimator;

    private float _timeToHold;
    private int _numSteps;
    
    private static readonly int ProgressNumber = Animator.StringToHash("ProgressNumber");

    public ExplosionSequence[] ExplosionPattern;

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
    /// <param name="numSteps"></param>
    /// <returns></returns>
    public Coroutine StartProgressBar(float timeToHold, int numSteps)
    {
        _timeToHold = timeToHold;
        _numSteps = numSteps;
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
        
        yield return new WaitForSeconds(_timeToHold / _numSteps);
        
        progressNumber++;
        if (progressNumber < _numSteps)
        {
            yield return ProgressBar(progressNumber);
        }
        else
        {
            progressBarAnimator.SetInteger(ProgressNumber, 0);
            progressBar.enabled = false;
            
            var inputState = LevelManager.Instance.GetInputState();
            switch (inputState)
            {
                case LevelManager.InputState.SelectPiece:
                    GameEvent.SelectPiece(this);
                    break;
                case LevelManager.InputState.SelectTarget:
                    GameEvent.AttackTarget(this);
                    break;
                case LevelManager.InputState.Attack:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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