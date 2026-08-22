using UnityEngine;

public class DrawingRigSpawner : MonoBehaviour
{
    [SerializeField] private LetterDrawing target;

    private DrawingRigReferences references;

    public LineRenderer Primary { get; private set; }
    public LineRenderer Secondary { get; private set; }

    public void Spawn()
    {
        references = ConfigManager.Instance.DrawingRig;

        Primary = SpawnLine(references.primaryLine);
        Secondary = SpawnLine(references.secondaryLine);

        target.lineRenderer = Primary;
        target.secondaryLineRenderer = Secondary;
    }

    private LineRenderer SpawnLine(DrawingRigReferences.LineEntry entry)
    {
        LineRenderer line = Instantiate(entry.prefab, Vector3.zero, Quaternion.identity);
        line.name = entry.instanceName;
        line.gameObject.layer = LayerMask.NameToLayer(entry.layerName);
        line.sortingOrder = entry.sortingOrder;
        line.numCapVertices = entry.capVertices;
        line.numCornerVertices = entry.cornerVertices;
        line.shadowCastingMode = entry.castShadows
            ? UnityEngine.Rendering.ShadowCastingMode.On
            : UnityEngine.Rendering.ShadowCastingMode.Off;

        return line;
    }
}
