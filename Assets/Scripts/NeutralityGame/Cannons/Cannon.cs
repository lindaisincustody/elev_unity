using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Cannonball Prefab")]
    public SoftBodyForceApplier cannonBall;

    [Header("Parameters")]
    public int shotAmount = 5;
    public float delayBetweenShots = 0.3f;
    public float cannonForce = 10f;
    public float rotationAngle = 15f;

    [Header("References")]
    public SoftbodyHolder holder;
    public Transform cannonBallHolder;
    public Transform exit;
    public Transform standingZone;

    private CannonAnimator anim;
    private Quaternion startingRot;

    private void Awake()
    {
        anim = GetComponentInChildren<CannonAnimator>();
    }

    void Start()
    {
        startingRot = transform.rotation;
    }

    public void Shoot(int ammount)
    {
        StartCoroutine(ShootCannon(ammount));
    }

    private IEnumerator ShootCannon(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            RotateCannon(Random.Range(-rotationAngle, rotationAngle));
            yield return StartCoroutine(anim.PlayShootAnimation());
            ShootCannonBall();
            yield return new WaitForSeconds(delayBetweenShots);
        }
        ResetRotation();
    }

    private void ShootCannonBall()
    {
        SoftBodyForceApplier newBall = Instantiate(cannonBall, exit.position, Quaternion.identity, cannonBallHolder);
        holder.softbodies.Add(newBall.transform.GetChild(0));
        newBall.ApplyForce(cannonForce, exit.up);
    }

    private void RotateCannon(float angle)
    {
        transform.Rotate(Vector3.forward, angle);
    }

    private void ResetRotation()
    {
        transform.rotation = startingRot;
    }
}
