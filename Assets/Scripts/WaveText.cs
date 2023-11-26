using System.Collections;
using TMPro;
using UnityEngine;

public class WaveText : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Coroutine WaveTextCoroutine() => StartCoroutine(WaveTextRoutine());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coroutine"></param>
    public void StopWaveText(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaveTextRoutine()
    {
        while (true)
        {
            textComponent.ForceMeshUpdate();
            var textInfo = textComponent.textInfo;

            for (var i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;
                
                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                
                for (var j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] =
                        orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 5f, 0);
                }
            }
            
            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textComponent.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }
}