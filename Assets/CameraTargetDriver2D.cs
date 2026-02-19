// Assets/Scripts/Camera/CameraTargetDriver2D.cs
using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public sealed class CameraTargetDriver2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera worldCamera;                 // DRAG your Base/Main world camera here
    [SerializeField] private CinemachineVirtualCamera vcam;      // DRAG your VCam here
    [SerializeField] private PolygonCollider2D bounds;           // Optional (for screen-edge confinement)

    [Header("Cursor Pull")]
    [Range(0f, 1f)][SerializeField] private float cursorWeight = 0.45f;
    [SerializeField] private float maxDistance = 5f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float smoothTimeFollow = 2f;

    [Header("Right-Click Zoom (Ortho Size)")]
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float zoomSpeed = 3f;

    [Header("Bounds Solver")]
    [SerializeField, Range(1, 16)] private int confineIterations = 8;
    [SerializeField] private float confineEpsilon = 0.001f;

    private float defaultOrthoSize;
    private Vector3 velocity;

    private void Awake()
    {
        if (player == null && Player.instance != null)
            player = Player.instance.transform;

        if (player == null || worldCamera == null || vcam == null)
        {
            Debug.LogError("Assign player, worldCamera (Base), and vcam in inspector.", this);
            enabled = false;
            return;
        }

        defaultOrthoSize = vcam.m_Lens.OrthographicSize;

        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        // 1) Zoom
        float targetSize = Input.GetMouseButton(1) ? defaultOrthoSize + zoomAmount : defaultOrthoSize;
        vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        bool rmb = Input.GetMouseButton(1);

        // 2) Compute desired camera-center target
        Vector3 desiredPos = rmb ? player.position : ComputeMousePulledTarget();

        desiredPos.z = transform.position.z;

        // 3) Confine view (optional, removes seeing outside)
        if (bounds != null && worldCamera.orthographic)
            desiredPos = ConfineScreenEdges(desiredPos);

        // 4) Smooth
        float t = rmb ? smoothTimeFollow : smoothTime;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, Mathf.Max(0.0001f, t));
    }

    private Vector3 ComputeMousePulledTarget()
    {
        // Raycast mouse onto plane at player's Z (robust for ortho + UI stacks)
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, player.position.z));
        Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

        if (!plane.Raycast(ray, out float enter))
            return player.position;

        Vector3 mouseWorld = ray.GetPoint(enter);

        Vector3 mid = Vector3.Lerp(player.position, mouseWorld, cursorWeight);

        Vector3 offset = mid - player.position;
        if (offset.magnitude > maxDistance)
            mid = player.position + offset.normalized * maxDistance;

        return mid;
    }

    private Vector3 ConfineScreenEdges(Vector3 cameraCenter)
    {
        for (int i = 0; i < confineIterations; i++)
        {
            Vector2 bl = Corner(cameraCenter, 0f, 0f);
            Vector2 br = Corner(cameraCenter, 1f, 0f);
            Vector2 tl = Corner(cameraCenter, 0f, 1f);
            Vector2 tr = Corner(cameraCenter, 1f, 1f);

            Vector2 corr =
                CornerCorrection(bl) +
                CornerCorrection(br) +
                CornerCorrection(tl) +
                CornerCorrection(tr);

            corr *= 0.25f;

            if (corr.sqrMagnitude <= confineEpsilon * confineEpsilon)
                break;

            cameraCenter = new Vector3(cameraCenter.x + corr.x, cameraCenter.y + corr.y, cameraCenter.z);
        }

        return cameraCenter;
    }

    private Vector2 Corner(Vector3 cameraCenter, float vx, float vy)
    {
        float halfH = worldCamera.orthographicSize;
        float halfW = halfH * worldCamera.aspect;

        float x = cameraCenter.x + Mathf.Lerp(-halfW, halfW, vx);
        float y = cameraCenter.y + Mathf.Lerp(-halfH, halfH, vy);
        return new Vector2(x, y);
    }

    private Vector2 CornerCorrection(Vector2 corner)
    {
        if (bounds.OverlapPoint(corner)) return Vector2.zero;
        Vector2 closest = bounds.ClosestPoint(corner);
        return closest - corner;
    }
}
