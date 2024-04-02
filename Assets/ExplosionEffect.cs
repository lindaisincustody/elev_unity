using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public Material explosionMaterial;
    private float progress = 0f;
    public float speed = 1f;

    void Update()
    {
        progress += Time.deltaTime * speed;
        if (progress > 1f)
        {
            Destroy(gameObject); // Destroy the object after the effect is done
            return;
        }
        explosionMaterial.SetFloat("_Progress", progress);
    }



    void OnDestroy()
    {
        explosionMaterial.SetFloat("_Progress", 0f); // Reset progress on destroy
    }
}