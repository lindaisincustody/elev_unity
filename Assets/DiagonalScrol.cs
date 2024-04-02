using UnityEngine;
using UnityEngine.UI;

public class DiagonalScrollUI : MonoBehaviour
{
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;
    private RawImage rawImage;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        // Update the uvRect position
        float newPosX = (rawImage.uvRect.x + scrollSpeedX * Time.deltaTime) % 1;
        float newPosY = (rawImage.uvRect.y + scrollSpeedY * Time.deltaTime) % 1;

        rawImage.uvRect = new Rect(newPosX, newPosY, rawImage.uvRect.width, rawImage.uvRect.height);
    }
}
