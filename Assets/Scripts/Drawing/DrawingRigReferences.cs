using UnityEngine;

[CreateAssetMenu(fileName = "DrawingRigReferences", menuName = "Custom/Drawing Rig References")]
public class DrawingRigReferences : ScriptableObject
{
    [System.Serializable]
    public class LineEntry
    {
        public LineRenderer prefab;
        public string instanceName = "LineRenderer";
        public string layerName = "Drawing";
        public int sortingOrder;
        public int capVertices = 90;
        public int cornerVertices = 90;
        public bool castShadows;
    }

    public LineEntry primaryLine = new LineEntry
    {
        instanceName = "LineRenderer",
        layerName = "Drawing",
        sortingOrder = 0,
    };

    public LineEntry secondaryLine = new LineEntry
    {
        instanceName = "LineRendererSecondary",
        layerName = "Drawing",
        sortingOrder = -1,
    };
}
