using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDrawingRig
{
    public LineRenderer Primary { get; private set; }
    public LineRenderer Secondary { get; private set; }
    public DrawingCameraRig Cameras { get; private set; }

    public void Spawn()
    {
        DrawingRigReferences references = ConfigManager.Instance.DrawingRig;

        Primary = SpawnLine(references.primaryLine);
        Secondary = SpawnLine(references.secondaryLine);

        Cameras = Object.Instantiate(references.cameraRig, Vector3.zero, Quaternion.identity);
        Cameras.name = references.cameraRig.name;
    }

    private LineRenderer SpawnLine(DrawingRigReferences.LineEntry entry)
    {
        LineRenderer line = Object.Instantiate(entry.prefab, Vector3.zero, Quaternion.identity);
        line.name = entry.instanceName;
        line.gameObject.layer = LayerMask.NameToLayer(entry.layerName);
        line.sortingOrder = entry.sortingOrder;
        line.numCapVertices = entry.capVertices;
        line.numCornerVertices = entry.cornerVertices;
        line.shadowCastingMode = entry.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;

        return line;
    }
}
