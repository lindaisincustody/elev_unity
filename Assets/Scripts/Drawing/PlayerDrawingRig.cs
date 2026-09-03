using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDrawingRig
{
    public LineRenderer Primary { get; private set; }
    public DrawingCameraRig Cameras { get; private set; }
    public Dictionary<DrawingMode, LineRenderer> ModeLines { get; private set; }

    public void Spawn()
    {
        DrawingRigReferences references = ConfigManager.Instance.DrawingRig;
        DrawingConfig config = ConfigManager.Instance.Drawing;

        Primary = SpawnLine(references.primaryLine);

        ModeLines = new Dictionary<DrawingMode, LineRenderer>();
        foreach (DrawingConfig.ModeEntry entry in config.Modes)
            ModeLines[entry.mode] = SpawnModeLine(entry);

        Cameras = Object.Instantiate(references.cameraRig, Vector3.zero, Quaternion.identity);
        Cameras.name = references.cameraRig.name;
        Object.DontDestroyOnLoad(Cameras.gameObject);
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
        line.useWorldSpace = true;
        Object.DontDestroyOnLoad(line.gameObject);

        return line;
    }

    private LineRenderer SpawnModeLine(DrawingConfig.ModeEntry entry)
    {
        LineRenderer line = Object.Instantiate(entry.linePrefab, Vector3.zero, Quaternion.identity);
        line.name = entry.linePrefab.name;
        line.useWorldSpace = true;
        line.positionCount = 0;
        line.enabled = false;
        Object.DontDestroyOnLoad(line.gameObject);

        return line;
    }
}
