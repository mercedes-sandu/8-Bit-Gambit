using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void AnimationComplete()
    {
        if (gameObject.activeSelf)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
