using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class RoomDoor : Interactable
{
    [SerializeField] private SaveId doorId;
    [SerializeField] private RoomId room;
    [SerializeField] private float length = 2f;
    [SerializeField] private float reach = 0.8f;
    [SerializeField] private float duration = 0.9f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [SerializeField] private BoxCollider2D zone;
    [SerializeField] private BoxCollider2D blocker;
    [SerializeField] private SpriteRenderer upperPanel;
    [SerializeField] private SpriteRenderer lowerPanel;
    [SerializeField] private Transform upperMask;
    [SerializeField] private Transform lowerMask;
    [SerializeField] private Transform upperHinge;
    [SerializeField] private Transform lowerHinge;

    private RoomsSnapshot snapshot;
    private float closedness = 1f;

    private bool IsUnlocked => RoomManager.Instance.IsUnlocked(room);
    private bool IsOpened => snapshot.OpenedDoors.Contains(doorId.Value);
    private bool CanOpen => IsUnlocked && !IsOpened;

    protected override void Start()
    {
        base.Start();
        Layout();

        snapshot = SaveLoadService.Instance.Get<GeneralSaveFile>().RoomsSnapshot;
        RoomManager.Instance.OnRoomChanged += HandleRoomChanged;

        SetClosedness(IsUnlocked && IsOpened ? 0f : 1f);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.Kill(this);

        RoomManager.Instance.OnRoomChanged -= HandleRoomChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerIsInTrigger = true;
        interactPrompt.Show(CanOpen);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerIsInTrigger = false;
        interactPrompt.Show(false);
    }

    protected override void HandleInteract()
    {
        if (!playerIsInTrigger || !CanOpen)
            return;

        base.HandleInteract();

        snapshot.OpenedDoors.Add(doorId.Value);
        SaveLoadService.Instance.SaveProgress();

        MoveTo(0f);
    }

    private void HandleRoomChanged(RoomId changed, bool unlocked)
    {
        if (changed != room)
            return;

        MoveTo(unlocked && IsOpened ? 0f : 1f);

        if (playerIsInTrigger)
            interactPrompt.Show(CanOpen);
    }

    private void MoveTo(float target)
    {
        DOTween.Kill(this);

        DOTween.To(() => closedness, SetClosedness, target, duration)
            .SetEase(ease)
            .SetTarget(this);
    }

    private void SetClosedness(float value)
    {
        closedness = value;

        float half = length / 2f;
        float slide = (1f - value) * half;
        bool visible = value > 0f;

        upperPanel.transform.localPosition = new Vector3(0f, half / 2f + slide, 0f);
        lowerPanel.transform.localPosition = new Vector3(0f, -half / 2f - slide, 0f);

        upperPanel.gameObject.SetActive(visible);
        lowerPanel.gameObject.SetActive(visible);
        blocker.enabled = visible;
    }

    [Button]
    private void Layout()
    {
        float half = length / 2f;
        Vector3 maskScale = new Vector3(upperPanel.size.x, half, 1f);

        zone.size = new Vector2(reach * 2f, length);
        blocker.size = new Vector2(blocker.size.x, length);

        upperPanel.size = new Vector2(upperPanel.size.x, half);
        lowerPanel.size = new Vector2(lowerPanel.size.x, half);
        upperMask.localPosition = new Vector3(0f, half / 2f, 0f);
        lowerMask.localPosition = new Vector3(0f, -half / 2f, 0f);
        upperMask.localScale = maskScale;
        lowerMask.localScale = maskScale;
        upperHinge.localPosition = new Vector3(0f, half, 0f);
        lowerHinge.localPosition = new Vector3(0f, -half, 0f);

        SetClosedness(closedness);
    }
}
