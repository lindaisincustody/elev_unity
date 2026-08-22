using UnityEngine;

[ExecuteInEditMode]
public class RipplePostProcessing : MonoBehaviour
{
    public Material rippleMaterial;  // Reference to the ripple material

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("OnRenderImage called"); // Check if this method is being triggered

        if (rippleMaterial != null)
        {
            Graphics.Blit(source, destination, rippleMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
