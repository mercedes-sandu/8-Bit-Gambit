using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    
    private InputState _inputState = InputState.SelectPiece;
    
    /// <summary>
    /// 
    /// </summary>
    public enum InputState
    {
        SelectPiece, SelectTarget, Attack
    }
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public InputState GetInputState() => _inputState;
}
