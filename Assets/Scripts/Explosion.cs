using UnityEngine;

public class Explosion : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void AnimationComplete()
    {
        if (!gameObject.activeSelf) return;
        
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}