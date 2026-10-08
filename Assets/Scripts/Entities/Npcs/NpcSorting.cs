using UnityEngine;

public class NpcSorting : Component
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private string elevatorLayer = "Objects";
    [SerializeField] private int elevatorOrder = 2;

    public void ChangeLayer()
    {
        sprite.sortingLayerName = elevatorLayer;
        sprite.sortingOrder = elevatorOrder;
    }

    public void HideOutsideMask()
    {
        sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
}
