using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private static readonly List<string> LevelSceneNames = new ()
    {
        "Level 1", "Level 2", "Level 3", "Level 4", "Level 5"
    };
    
    private static string _levelNameToLoad = "Level 1";
    
    /// <summary>
    /// Subscribes to the OnLevelOver event and sets this object to not be destroyed on scene load.
    /// Note: This must be present in the first level of the game.
    /// </summary>
    private void Start()
    {
        GameEvent.OnLevelOver += LevelOver;
        
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Called when a level has been completed by the player. Either loads proper UI canvases or loads the game over
    /// scene. 
    /// </summary>
    /// <param name="endState">Either the player won or lost the level, or there was a draw.</param>
    /// <param name="nextLevelName"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void LevelOver(LevelManager.EndState endState, string nextLevelName)
    {
        _levelNameToLoad = nextLevelName;
        switch (endState)
        {
            case LevelManager.EndState.PlayerWon:
                LevelManager.Instance.LevelWon();
                break;
            case LevelManager.EndState.PlayerLost:
                LevelManager.Instance.LevelLost();
                break;
            case LevelManager.EndState.Draw:
                LevelManager.Instance.LevelDraw();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(endState), endState, null);
        }
    }

    /// <summary>
    /// Loads the level corresponding to the current level index, called after LevelOver by buttons in level over
    /// canvases in InGameUI.
    /// </summary>
    public static void LoadLevel()
    {
        Debug.Log($"loading {_levelNameToLoad}");
        SceneManager.LoadScene(_levelNameToLoad);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnLevelOver -= LevelOver;
    }
}