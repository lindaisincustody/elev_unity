using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleHandler : MonoBehaviour
{
    [Range(5, 15)]
    public float radius = 15;
    public float maxDistance;
    public float minDistance;
    public float minOffset;
    public float maxOffset;
    public GameObject tenctacle;

    public List<GameObject> tentacles = new List<GameObject>();
    private List<float> offsets = new List<float>();

    private float degrees;

    private void Start()
    {
        for (int i = 0; i < tentacles.Count; i++)
        {
            offsets.Add(0f);
            offsets[i] = Random.Range(minOffset, maxOffset);
        }
        OnRadiusChanged();
    }

    private void Update()
    {
        OnRadiusChanged();
    }

    private void OnRadiusChanged()
    {
        degrees = 360f / tentacles.Count;
        for (int i = 0; i < tentacles.Count; i++)
        {
            // Calculate the angle for each tentacle
            float angle = degrees * i * Mathf.Deg2Rad; // Convert degrees to radians

            // Calculate the position on the circle
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * (radius + offsets[i]), // X position
                Mathf.Sin(angle) * (radius + offsets[i]), // Y position
                0                          // Z position (2D game)
            );

            // Set the position of the tentacle
            if (tentacles[i] != null)
            {
                tentacles[i].transform.position = position + transform.position; // Adjust position relative to the parent object

                // Rotate to face the center of the circle
                float angleDegrees = degrees * i - 180; // Subtract 90 degrees to align correctly
                tentacles[i].transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
            }
        }
    }
}
