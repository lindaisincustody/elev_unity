using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public SoftBodyForceApplier cannonBall;
    public Transform cannonBallHolder;
    public Transform exit;
    public int shotAmount = 5;
    public float delayBetweenShots = 0.3f;
    public float rotationAngle = 15f; // Adjust the rotation angle based on your preference
    public float cannonForce = 10f;

    private Quaternion startingRot;
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        startingRot = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Shoot();
    }

    private void Shoot()
    {
        StartCoroutine(ShootCannon(shotAmount));
    }

    private IEnumerator ShootCannon(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            RotateCannon(Random.Range(-rotationAngle, rotationAngle));
            PlayShootAnimation();
            ShootCannonBall();
            yield return new WaitForSeconds(delayBetweenShots);
        }
        ResetRotation();
    }

    private void ShootCannonBall()
    {
        SoftBodyForceApplier newBall = Instantiate(cannonBall, exit.position, Quaternion.identity, cannonBallHolder);
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

    private void PlayShootAnimation()
    {
        m_Animator.SetTrigger("Shoot");
    }
}
