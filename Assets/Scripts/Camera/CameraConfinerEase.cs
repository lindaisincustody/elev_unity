using Cinemachine;
using UnityEngine;

public class CameraConfinerEase : CinemachineExtension
{
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float jumpSpeed = 20f;

    private Vector3 targetCorrection;
    private Vector3 appliedCorrection;
    private Vector3 easeStart;
    private float elapsed;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Noise)
            return;

        Vector3 correction = state.PositionCorrection;

        if (!vcam.PreviousStateIsValid || deltaTime < 0f)
        {
            targetCorrection = correction;
            appliedCorrection = correction;
            elapsed = duration;
            return;
        }

        if ((correction - targetCorrection).magnitude > jumpSpeed * deltaTime)
        {
            easeStart = appliedCorrection;
            elapsed = 0f;
        }
        targetCorrection = correction;

        elapsed = Mathf.Min(elapsed + deltaTime, duration);
        float eased = (1f - Mathf.Cos(Mathf.PI * elapsed / duration)) * 0.5f;
        appliedCorrection = Vector3.Lerp(easeStart, correction, eased);

        state.PositionCorrection = appliedCorrection;
    }
}
