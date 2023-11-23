using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;

    private bool _isPlayerTurn = true;
    
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        GameEvent.OnTurnComplete += UpdateTurnText;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void UpdateTurnText()
    {
        _isPlayerTurn = !_isPlayerTurn;
        turnText.text = _isPlayerTurn ? "Player's Turn" : "Opponent's Turn";
    }
}