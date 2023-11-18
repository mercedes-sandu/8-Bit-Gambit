using UnityEngine;

[CreateAssetMenu(fileName = "New Sequence", menuName = "Explosion Sequence")]
public class ExplosionSequence : ScriptableObject
{
    // Sequence
    // - Array of numbers from 1 -> tileNumEdges
    // - Any value greater than tileNumEdges indicates to SKIP that tile for damage
    // - eg 1, 2, 3, 4 = edges for square tiles; 5, 6, 7, 8 = skip either 1, 2, 3, 4 while building sequence
    // If a Sequence repeats, simply set IsRepeating
    // - eg Bishop might use [6, 1] to say SKIP right, USE up, then repeat across all available tiles
    public int[] Sequence;
    public bool IsRepeating = false;
}
