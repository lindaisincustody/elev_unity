using UnityEngine;

public class InteractPrompt : Component
{
    [SerializeField] private GameObject icon;

    public void Show(bool show)
    {
        if (show == icon.activeSelf)
            return;

        SoundManager.PlaySound2D(SoundManager.Sound.QuietClick, 0.7f);
        icon.SetActive(show);
    }
}
