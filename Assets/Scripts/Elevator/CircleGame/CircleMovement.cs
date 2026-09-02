using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private RectTransform ring;

    [SerializeField] private HollowCircleManager mngr;

    [Header("Movement Settings")] public float rotationSpeed = 3f;

    public float orbitRadius = 365f;
    public float hitAngleTolerance = 17f;

    public bool isActive = false;

    private RectTransform rect;
    private float currentAngle;

    private void Awake()
    {
        rect = (RectTransform)transform;
        UpdateOrbitPosition();
    }

    private void OnEnable()
    {
        currentAngle = 0f;
        UpdateOrbitPosition();
    }

    private void Update()
    {
        if (!isActive)
            return;

        currentAngle += rotationSpeed * Time.deltaTime;
        UpdateOrbitPosition();

        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        HollowCircle target = mngr.FindCircleAtAngle(currentAngle * Mathf.Rad2Deg, hitAngleTolerance);

        if (target == null)
            mngr.MissAnimation();
        else
            target.HitHollowCircle();
    }

    private void UpdateOrbitPosition()
    {
        Vector2 direction = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
        rect.anchoredPosition = ring.anchoredPosition + direction * orbitRadius;
    }
}
