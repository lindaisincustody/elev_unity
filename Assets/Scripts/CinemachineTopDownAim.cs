using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public sealed class CinemachineTopDownAim_CM2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Cursor Pull")]
    [Range(0f, 1f)]
    [SerializeField] private float cursorWeight = 0.45f;
    [SerializeField] private float maxDistance = 5f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float smoothTimeFollow = 2f;

    [Header("Right-Click Zoom (Ortho Size)")]
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float zoomSpeed = 3f;

    private Camera unityCam;
    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer framing;

    private float defaultOrthoSize;

    private Vector3 offsetVelocity;
    private Vector3 currentOffset;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();

        if (player == null && Player.instance != null)
            player = Player.instance.transform;

        vcam.Follow = player;

        framing = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framing == null)
        {
            Debug.LogError(
                "Framing Transposer not found. On the vcam, set Body = Framing Transposer.",
                this
            );
            enabled = false;
            return;
        }

        unityCam = Camera.main;
        defaultOrthoSize = vcam.m_Lens.OrthographicSize;

        currentOffset = framing.m_TrackedObjectOffset;
    }

    private void Update()
    {
        if (player == null || unityCam == null) return;

        // 1) Zoom
        float targetSize = Input.GetMouseButton(1) ? defaultOrthoSize + zoomAmount : defaultOrthoSize;
        vcam.m_Lens.OrthographicSize = Mathf.Lerp(
            vcam.m_Lens.OrthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );

        // 2) Desired offset
        bool rmb = Input.GetMouseButton(1);

        Vector3 desiredOffset;
        float dampTime;

        if (rmb)
        {
            desiredOffset = Vector3.zero;
            dampTime = smoothTimeFollow;
        }
        else
        {
            Vector3 mouseWorld = unityCam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = player.position.z;

            Vector3 targetPos = Vector3.Lerp(player.position, mouseWorld, cursorWeight);

            Vector3 offset = targetPos - player.position;
            if (offset.magnitude > maxDistance)
                offset = offset.normalized * maxDistance;

            desiredOffset = offset;
            dampTime = smoothTime;
        }

        // 3) Smooth the offset
        currentOffset = Vector3.SmoothDamp(
            currentOffset,
            desiredOffset,
            ref offsetVelocity,
            Mathf.Max(0.0001f, dampTime)
        );

        currentOffset.z = 0f;
        framing.m_TrackedObjectOffset = currentOffset;

        // Optional: tie Cinemachine damping to mode (prevents “floaty” feel differences)
        float d = Mathf.Clamp(dampTime, 0f, 10f);
        framing.m_XDamping = d;
        framing.m_YDamping = d;
    }
}
