using UnityEngine;

public class MirrorReflection : MonoBehaviour
{
    public Camera mainCamera;
    public Camera reflectionCamera;
    public Transform mirrorTransform;
    public Vector3 camToMirror;
    public Vector3 mirrorNormal;
    public Vector3 reflectionPosition;
    public Vector3 forward;


    public float distanceOffset = 0f;
    public float distanceOffsetX = 0f;

    void LateUpdate()
    {
        mirrorNormal = mirrorTransform.up;
        camToMirror = mainCamera.transform.position - mirrorTransform.position;
        float distance = Vector3.Dot(camToMirror, mirrorNormal);
        reflectionPosition = mainCamera.transform.position - 2 * distance * mirrorNormal;

        reflectionPosition += mirrorNormal * distanceOffset;

        reflectionPosition.y = mirrorTransform.position.y - (reflectionPosition.y - mirrorTransform.position.y);

        reflectionCamera.transform.position = reflectionPosition;

        forward = mainCamera.transform.forward;
        Vector3 reflectedForward = Vector3.Reflect(forward, mirrorNormal);

        reflectionCamera.transform.rotation = Quaternion.LookRotation(reflectedForward, Vector3.up);
    }
}