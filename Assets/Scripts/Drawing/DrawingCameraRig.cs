using UnityEngine;

public class DrawingCameraRig : MonoBehaviour
{
    [field: SerializeField] public Camera MLCamera { get; private set; }
    [field: SerializeField] public Camera DisplayCamera { get; private set; }

    public void SetActive(bool active)
    {
        MLCamera.enabled = active;
        DisplayCamera.enabled = active;
    }
}
