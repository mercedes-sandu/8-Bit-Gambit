using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    // todo: remove "TestInput" from this list
    private static readonly List<string> LevelSceneNames = new ()
    {
        "TestInput", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5"
    };

    private const string GameEndSceneName = "GameOver";

    private static int _currentLevelIndex = 0;
    
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
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void LevelOver(LevelManager.EndState endState)
    {
        switch (endState)
        {
            case LevelManager.EndState.PlayerWon:
                _currentLevelIndex++;
                if (_currentLevelIndex >= LevelSceneNames.Count)
                {
                    _currentLevelIndex = 0;
                    SceneManager.LoadScene(GameEndSceneName);
                }
                else
                {
                    LevelManager.Instance.LevelWon();
                }
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
        SceneManager.LoadScene(LevelSceneNames[_currentLevelIndex]);
    }
}