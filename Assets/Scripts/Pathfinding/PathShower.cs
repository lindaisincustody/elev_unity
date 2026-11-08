using UnityEngine;

public class PathShower : MonoBehaviour
{
    [SerializeField] private Transform from;
    [SerializeField] private Transform to;

    public void Show()
    {
        PathGuide.Instance.Show(from.position, to.position);
    }
}
