using DG.Tweening;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private RoomId id;
    [SerializeField] private MeshRenderer blackoutRenderer;
    [SerializeField] private GameObject blackoutObject;

    private float fadeDuration = 1f;
    private Ease fadeEase = Ease.InOutSine;

    private readonly int FadeId = Shader.PropertyToID("_Fade");

    private MaterialPropertyBlock propertyBlock;
    private float fade;

    public bool IsUnlocked => RoomManager.Instance.IsUnlocked(id);

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();

        RoomManager.Instance.OnRoomChanged += HandleRoomChanged;

        ApplyInstantly(RoomManager.Instance.IsUnlocked(id));
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);

        RoomManager.Instance.OnRoomChanged -= HandleRoomChanged;
    }

    public void SetUnlocked(bool unlocked)
    {
        DOTween.Kill(this);

        if (unlocked)
            FadeOut();
        else
            FadeIn();
    }

    private void HandleRoomChanged(RoomId changed, bool unlocked)
    {
        if (changed != id)
            return;

        SetUnlocked(unlocked);
    }

    private void ApplyInstantly(bool unlocked)
    {
        blackoutObject.SetActive(!unlocked);

        SetFade(1f);
    }

    private void FadeOut()
    {
        DOTween.To(() => fade, SetFade, 0f, fadeDuration)
            .SetEase(fadeEase)
            .SetTarget(this)
            .OnComplete(() => blackoutObject.SetActive(false));
    }

    private void FadeIn()
    {
        blackoutObject.SetActive(true);

        SetFade(0f);

        DOTween.To(() => fade, SetFade, 1f, fadeDuration)
            .SetEase(fadeEase)
            .SetTarget(this);
    }

    private void SetFade(float value)
    {
        fade = value;

        blackoutRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FadeId, value);
        blackoutRenderer.SetPropertyBlock(propertyBlock);
    }
}
