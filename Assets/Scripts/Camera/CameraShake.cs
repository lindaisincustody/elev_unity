using Cinemachine;
using UnityEngine;

public class CameraShake : CinemachineExtension
{
    private float remaining;
    private float magnitude;

    public void Shake(float duration, float shakeMagnitude)
    {
        remaining = duration;
        magnitude = shakeMagnitude;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize || remaining <= 0f)
            return;

        remaining -= Time.deltaTime;

        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;

        state.PositionCorrection += new Vector3(x, y, 0f);
    }
}
