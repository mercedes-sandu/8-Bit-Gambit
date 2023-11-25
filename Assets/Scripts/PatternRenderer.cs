using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatternRenderer : MonoBehaviour
{
    [SerializeField] private GameObject patternHighlightPrefab;

    private ChessPiece _controllingPiece;
    private List<GameObject> _highlights = new(); // Might be simpler as a list of vec3
    private GameObject _highlightRoot;

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
    // -- (Not really sure how I'm gonna do this beyond 4 sided-tiles. Needs math)

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    public void SetControllingPiece(ChessPiece piece)
    {
        _controllingPiece = piece;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void TriggerAnimations()
    {
        transform.SetParent(null);
        foreach (var animator in from highlight in _highlights
                 select GetComponentsInChildren<Animator>()
                 into animators
                 from animator in animators
                 where animator
                 select animator)
        {
            animator.Play("ExplosionAnim");
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enableSpriteRenderer"></param>
    public void DrawPattern(bool enableSpriteRenderer)
    {
        // The ROOT highlight must only be instantiated once
        // ALL SEQUENCES start from the ROOT highlight
        // AFTER finding the transform following ROOT, all SEQUENCES build from last transform
        // Add new highlight to the list
        foreach (var pattern in _controllingPiece.ExplosionPattern)
        {
            if (!_highlightRoot)
            {
                // transform will always equal the parented object's transform
                // Attach the ROOT highlight to this transform
                // ONLY DO ONCE
                _highlightRoot = Instantiate(patternHighlightPrefab, transform, false);
                _highlightRoot.GetComponent<SpriteRenderer>().enabled = enableSpriteRenderer;
            }
            for (var i = 0; i < pattern.Sequence.Length; i++)
            {
                // Set the new transform to start from EITHER ROOT or LAST ADDED GAMEOBJECT
                var transformStart = i == 0 ? _highlightRoot.transform : _highlights.Last().transform;

                // Determine if this highlight is to be skipped
                const int numEdges = Board.NumEdgesInTile;
                var edgeNum = pattern.Sequence[i] - numEdges;
                var isSkipped = edgeNum > 0;
                var switchNum = pattern.Sequence[i];
                if (isSkipped)
                {
                    switchNum = edgeNum;
                }
                
                // Setup the new Transform
                var newX = transformStart.localPosition.x;
                var newY = transformStart.localPosition.y;
                switch (switchNum)
                {
                    case 1:
                        newY += 1;
                        break;
                    case 2:
                        newX += 1;
                        break;
                    case 3:
                        newY -= 1;
                        break;
                    case 4:
                        newX -= 1;
                        break;
                    default:
                        break;
                }
                
                // Create the new Transform from our calculations
                var newTransform = new Vector3(newX, newY, transform.position.z);
                
                // Create the new Highlight to be saved; parent to this GameObject
                var newHighlight = Instantiate(patternHighlightPrefab, transform, false);
                newHighlight.GetComponent<SpriteRenderer>().enabled = enableSpriteRenderer;
                
                // Adjust it's transform by our calculations
                newHighlight.transform.SetLocalPositionAndRotation(newTransform, transform.rotation);
                if (isSkipped)
                {
                    newHighlight.SetActive(false);
                }
                
                // Add to the list
                _highlights.Add(newHighlight);
            }
        }
    }
}