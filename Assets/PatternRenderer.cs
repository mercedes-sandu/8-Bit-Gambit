using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRenderer : MonoBehaviour
{
    public GameObject PatternHighlightPrefab;
    public ChessPiece Piece;
    private ExplosionSequence[] Pattern;

    private void Awake()
    {
        // Draw out the sequence
        if (Piece != null)
        {
            if (Piece.ExplosionPattern.Length > 0)
            {
                Pattern = Piece.ExplosionPattern;
            }
        }
    }

    private void Start()
    {
        // Draw the pattern on start
        // TODO:
        // - Add the initial prefab highlighting the root tile
        // - For each Sequence
        // -- For each entry in Sequence
        // -- Add the PatternHighlightPrefab as child
        // -- Adjust it's transform based on the Sequence entry
        // --- For a SQUARE board:
        // ---- If Sequence entry - numEdgesPerTile > 0; Add a blank gameobject as child
        // Revisit this logic, might be a cleaner way than added unused objects
        // ---- 2 == Add 1 to x; 4 == Add -1 to x
        // ---- 1 == Add 1 to y; 3 == Add -1 to y
        // -- (Not really sure how I'm gonna do this beyond 4 tiles. The math gets weird)
    }
}
