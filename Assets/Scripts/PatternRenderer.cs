using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatternRenderer : MonoBehaviour
{
    public GameObject PatternHighlightPrefab;
    public ChessPiece ControllingPiece;
    public Grid myGrid;
    private List<GameObject> Highlights = new(); // Might be simpler as a list of vec3
    private GameObject HighlightRoot;

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
    public void TriggerAnimations()
    {
        foreach (var animator in from highlight in Highlights
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
    public void DrawPattern()
    {
        // The ROOT highlight must only be instantiated once
        // ALL SEQUENCES start from the ROOT highlight
        // AFTER finding the transform following ROOT, all SEQUENCES build from last transform
        // Add new highlight to the list
        foreach (var pattern in ControllingPiece.ExplosionPattern)
        {
            if (!HighlightRoot)
            {
                // transform will always equal the parented object's transform
                // Attach the ROOT highlight to this transform
                // ONLY DO ONCE
                HighlightRoot = Instantiate(PatternHighlightPrefab, transform, false);
            }
            for (var i = 0; i < pattern.Sequence.Length; i++)
            {
                // Set the new transform to start from EITHER ROOT or LAST ADDED GAMEOBJECT
                var transformStart = i == 0 ? HighlightRoot.transform : Highlights.Last<GameObject>().transform;

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
                var newHighlight = Instantiate(PatternHighlightPrefab, transform, false);
                
                // Adjust it's transform by our calculations
                newHighlight.transform.SetLocalPositionAndRotation(newTransform, transform.rotation);
                if (isSkipped)
                {
                    newHighlight.SetActive(false);
                }
                
                // Add to the list
                Highlights.Add(newHighlight);
            }
        }
    }
}