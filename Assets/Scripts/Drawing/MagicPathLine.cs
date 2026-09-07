using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MagicPathLine : MonoBehaviour
{
    private static readonly int LineLengthId = Shader.PropertyToID("_LineLength");
    private static readonly int LineWidthId = Shader.PropertyToID("_LineWidth");

    [SerializeField] private float walkableWidthRatio = 0.72f;

    private LineRenderer line;
    private MaterialPropertyBlock block;

    public float WalkableWidth => line.widthMultiplier * walkableWidthRatio;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        block = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        line.GetPropertyBlock(block);
        block.SetFloat(LineLengthId, WorldLength());
        block.SetFloat(LineWidthId, line.widthMultiplier);
        line.SetPropertyBlock(block);
    }

    private float WorldLength()
    {
        float length = 0f;

        for (int i = 1; i < line.positionCount; i++)
            length += Vector3.Distance(line.GetPosition(i - 1), line.GetPosition(i));

        return length;
    }
}
