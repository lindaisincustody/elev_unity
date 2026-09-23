using UnityEngine;

public class ItemInteractable : MonoBehaviour
{
    [SerializeField] private SaveId pickupId;
    [SerializeField] private Item item;

    private PickupsSnapshot snapshot;

    private void Start()
    {
        snapshot = SaveLoadService.Instance.Get<GeneralSaveFile>().PickupsSnapshot;

        if (snapshot.IsCollected(pickupId.Value))
        {
            gameObject.SetActive(false);
            return;
        }

        ItemDropper.Instance.Spawn(item, transform.position).OnPicked += HandlePicked;
    }

    private void HandlePicked()
    {
        snapshot.Collected.Add(pickupId.Value);
        SaveLoadService.Instance.SaveProgress();
    }
}
