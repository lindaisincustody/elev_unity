using UnityEngine;

public class NpcAnchor : MonoBehaviour
{
    [SerializeField] private string anchorId;

    public string AnchorId => anchorId;

    private void OnEnable()
    {
        NpcManager.Instance.RegisterAnchor(this);
    }

    private void OnDisable()
    {
        NpcManager.Instance.UnregisterAnchor(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
