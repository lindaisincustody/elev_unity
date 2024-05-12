using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WavyTextEffect : MonoBehaviour
{
    public float waveSpeed = 1.0f;
    public float waveFrequency = 5.0f;
    public float waveAmplitude = 5.0f;

    private TMP_Text tmpText;
    private Mesh mesh;
    private Vector3[] vertices;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        tmpText.ForceMeshUpdate(); // Forces the text mesh to be updated
        mesh = tmpText.mesh;
        vertices = mesh.vertices;

        int characterCount = tmpText.textInfo.characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = tmpText.textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            for (int j = 0; j < 4; j++) // Each character has 4 vertices
            {
                Vector3 orig = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * waveSpeed + orig.x * waveFrequency) * waveAmplitude, 0);
            }
        }

        mesh.vertices = vertices; // Apply the changes to the vertices
        tmpText.canvasRenderer.SetMesh(mesh);
    }
}
