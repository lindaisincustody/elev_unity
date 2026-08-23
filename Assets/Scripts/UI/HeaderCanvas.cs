using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderCanvas : MonoBehaviour
{
    [field: SerializeField] public RawImage DrawingDisplay { get; private set; }
    [field: SerializeField] public RawImage RenderTextureDisplay { get; private set; }
}
