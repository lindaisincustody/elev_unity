using UnityEngine;

public class DrawingRigSpawner : MonoBehaviour
{
    private DrawingRigReferences references;

    public LineRenderer Primary { get; private set; }
    public LineRenderer Secondary { get; private set; }
    public bool HasSpawned => Primary != null && Secondary != null;

    public void Spawn()
    {
            
        references = ConfigManager.Instance.DrawingRig;

        LetterDrawing target = Player.instance.Get<LetterDrawing>();

        Primary = SpawnLine(references.primaryLine);
        Secondary = SpawnLine(references.secondaryLine);

        target.lineRenderer = Primary;
        target.secondaryLineRenderer = Secondary;
    }

    private LineRenderer SpawnLine(DrawingRigReferences.LineEntry entry)
    {
        LineRenderer line = Instantiate(entry.prefab, Vector3.zero, Quaternion.identity);
        line.name = string.IsNullOrEmpty(entry.instanceName) ? entry.prefab.name : entry.instanceName;

        int layer = LayerMask.NameToLayer(entry.layerName);
        if (layer < 0)
            Debug.LogError($"[DrawingRigSpawner] Layer \"{entry.layerName}\" does not exist — " +
                           $"'{line.name}' stays on layer {line.gameObject.layer}.", this);
        else
            line.gameObject.layer = layer;

        line.sortingOrder = entry.sortingOrder;
        line.numCapVertices = entry.capVertices;
        line.numCornerVertices = entry.cornerVertices;
        line.shadowCastingMode = entry.castShadows
            ? UnityEngine.Rendering.ShadowCastingMode.On
            : UnityEngine.Rendering.ShadowCastingMode.Off;

        return line;
    }
}
