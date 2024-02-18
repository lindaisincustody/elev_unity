using UnityEngine;

public class GunPositioner : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 10f;
    public float offsetDistance = 1f;
    [SerializeField] InputManager inputManager;

    private float angleSpeed = 0;
    private float angle = 0;

    private void Awake()
    {
        inputManager.OnRLeft += RotateGunLeft;
        inputManager.OnRLeftCancel += StopRotateGunLeft;
        inputManager.OnLLeft += StopRotateGunLeft;
        inputManager.OnLLeftCancel += RotateGunLeft;
    }

    private void RotateGunLeft()
    {
        angleSpeed += rotationSpeed;
    }

    private void StopRotateGunLeft()
    {
        angleSpeed -= rotationSpeed;
    }


    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in GunController script!");
            return;
        }
        angle += angleSpeed;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        transform.position = player.position + (rotation * Vector3.right * offsetDistance);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        //ImageFlipCheck(transform.rotation.x);
    }

    private void ImageFlipCheck(float x)
    {
        if (x == 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
