using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Item _item;
    private bool _picked;

    public void SetItem(Item item)
    {
        _item = item;

        spriteRenderer.sprite = item.sprite;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (_picked)
            return;
        
        Player.instance.Get<ItemsInventory>().AddItem(_item);
        _picked = true;

        DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * 1.4f, .2f).SetEase(Ease.OutSine))
            .Append(transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InSine))
            .OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
