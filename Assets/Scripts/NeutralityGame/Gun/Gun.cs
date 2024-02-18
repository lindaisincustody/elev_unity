using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private int suckInPower;
    [SerializeField] private int shootPower;
    [SerializeField] private Transform cannonBallHolder;
    [SerializeField] private LayerMask consumableLayer;
    [SerializeField] private SpriteRenderer liquidRenderer;
    [SerializeField] private SoftBody[] softbodies;
    [SerializeField] private SuckInEffect suckIneffect;
    [SerializeField] private Transform gunEnd;
    [SerializeField] private InputManager playerInput;
    private SoftBody.Shape suckedInShape;
    private bool canShoot = false;
    private SoftbodySucker sucker;

    private void Awake()
    {
        playerInput.OnFire += Shoot;
    }

    private void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        sucker = new SoftbodySucker();
        suckIneffect.ActivateWind();
    }

    private void Shoot()
    {
        if (canShoot)
        {
            ShootSoftBody();
        }
    }

    private IEnumerator ShootWindStream()
    {
        yield return new WaitForSeconds(1f);
        suckIneffect.ActivateWind();
    }

    public void SuckInSoftBody(SoftBody softbody)
    {
        suckedInShape = softbody.softbodyShape;
        SoftBodyForceApplier forceApplier = softbody.GetComponent<SoftBodyForceApplier>();
        sucker.SuckIn(forceApplier, transform, suckInPower, liquidRenderer, OnSuckComplete);
        suckIneffect.ActivateSuckInEffect(softbody.transform.GetChild(0));
    }

    private void ShootSoftBody()
    {
        canShoot = false;
        SoftBodyForceApplier newBall = Instantiate(GetSuckedInShape(), gunEnd.position, Quaternion.identity, cannonBallHolder).GetComponent<SoftBodyForceApplier>();
        newBall.GetComponent<ColorAssigner>().AssignBrightColor(sucker.GetColor());
        sucker.ShootSoftBody(newBall, shootPower, transform.right);
        liquidRenderer.color = Color.white;
        StartCoroutine(ShootWindStream());
    }

    private SoftBody GetSuckedInShape()
    {
        return softbodies[(int)suckedInShape];
    }

    private void OnSuckComplete()
    {
        canShoot = true;
    }
}
