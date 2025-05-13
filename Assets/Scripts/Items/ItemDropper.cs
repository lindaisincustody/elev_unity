using UnityEngine;
using DG.Tweening;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] private PickUp pickUp;
    [SerializeField] private Item item;

    private float _duration = 1f;

    public void Drop(Vector3 position)
    {   
        PickUp newPickup = Instantiate(pickUp, position, Quaternion.identity);
        newPickup.SetItem(item);

        newPickup.transform.localScale = Vector3.zero;

        newPickup.transform.DOScale(Vector3.one, _duration / 5f).SetEase(Ease.InSine);
        newPickup.transform.DOJump(position, 2, 1, _duration);
    }
}
