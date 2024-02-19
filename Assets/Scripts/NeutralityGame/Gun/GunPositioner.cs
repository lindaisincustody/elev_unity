using UnityEngine;

public class GunPositioner : MonoBehaviour
{
    [Header("Parameters")]
    public float rotationSpeed = 10f;
    public float offsetDistance = 1f;

    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] InputManager playerInput;
    [SerializeField] SoftbodyHolder softbodyHolder;
    [SerializeField] SoftbodyFollower softbodyFollower;
    private bool _canShoot = false;
    public bool canShoot
    {
        get { return _canShoot; }
        private set
        {
            if (_canShoot != value && !value) // If canShoot changes to false
            {
                _canShoot = value;
                GetClosestSoftBody();
            }
            else
            {
                _canShoot = value;
            }
        }
    }

    private float angle;
    private bool rotateClockwise = false;
    private float constantAngle;
    private Transform targetSoftbody;

    private void Start()
    {
        constantAngle = rotationSpeed;
        playerInput.OnNext += GetClosestSoftBody;
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in GunController script!");
            return;
        }

        if (canShoot)
            RotateGunForShooting();
        else if (targetSoftbody != null)
            RotateToTarget();

        ImageFlipCheck(transform.rotation.z);
    }

    private void GetClosestSoftBody()
    {
        if (canShoot)
            return;
        targetSoftbody = softbodyHolder.FindClosestSoftbody(transform);
        softbodyFollower.SetTarget(targetSoftbody);
    }

    private void RotateToTarget()
    {
        Vector3 direction = (targetSoftbody.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, constantAngle * Time.deltaTime);
    }

    private void RotateGunForShooting()
    {
        angle += rotationSpeed * Time.deltaTime;
        if (!rotateClockwise && transform.rotation.z > 0.9f)
        {
            rotateClockwise = true;
            rotationSpeed *= -1;
        }
        if (rotateClockwise && transform.rotation.z < 0.4f)
        {
            rotateClockwise = false;
            rotationSpeed *= -1;
        }

        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        transform.position = player.position + (rotation * Vector3.right * offsetDistance);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, constantAngle * Time.deltaTime);
    }

    private void ImageFlipCheck(float x)
    {
        if (x > 0.7f)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void SetCanShoot(bool newValue)
    {
        canShoot = newValue;
    }

    private void OnDestroy()
    {
        playerInput.OnNext -= GetClosestSoftBody;
    }
}
