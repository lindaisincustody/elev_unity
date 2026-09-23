using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class ItemDropper : CoreService
{
    public static ItemDropper Instance { get; private set; }

    private float _duration = 1f;

    public override UniTask Initialize()
    {
        Instance = this;

        return UniTask.CompletedTask;
    }

    public ItemPickup Spawn(Item item, Vector3 position)
    {
        ItemPickup newPickup = Instantiate(ConfigManager.Instance.References.ItemPickup, position, Quaternion.identity);
        newPickup.SetItem(item);

        return newPickup;
    }

    public void Drop(Item item, Vector3 position)
    {
        ItemPickup newPickup = Spawn(item, position);

        newPickup.transform.localScale = Vector3.one * 0.2f;
        newPickup.transform.DOScale(Vector3.one, 0.2f);
        newPickup.transform.DOJump(position, .2f, 1, _duration);
    }
}
