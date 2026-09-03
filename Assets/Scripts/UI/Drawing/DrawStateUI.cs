using UnityEngine;
using UnityEngine.UI;

public class DrawStateUI : MonoBehaviour
{
    [SerializeField] private GameObject drawStateHolder;
    [SerializeField] private Image drawStateImage;

    private LetterDrawing letterDrawing;

    private void Start()
    {
        letterDrawing = Player.instance.Get<LetterDrawing>();
        letterDrawing.OnModeChanged += Apply;

        Apply();
    }

    private void OnDestroy()
    {
        letterDrawing.OnModeChanged -= Apply;
    }

    private void Apply()
    {
        bool hasMode = letterDrawing.CurrentMode.HasValue;

        drawStateHolder.SetActive(hasMode);

        if (hasMode)
            drawStateImage.sprite = letterDrawing.CurrentSprite;
    }
}
