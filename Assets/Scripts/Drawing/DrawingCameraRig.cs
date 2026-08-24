using UnityEngine;

public class DrawingCameraRig : MonoBehaviour
{
    [field: SerializeField] public Camera MLCamera { get; private set; }
    [field: SerializeField] public Camera DisplayCamera { get; private set; }
}
