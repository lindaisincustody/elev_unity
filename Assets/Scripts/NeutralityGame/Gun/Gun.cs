using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private int suckInPower;
    [SerializeField] private int shootPower;
    [SerializeField] private LayerMask consumableLayer;

    [Header("References")]
    [SerializeField] private Transform cannonBallHolder;
    [SerializeField] private SuckInEffect suckIneffect;
    [SerializeField] private Transform gunEnd;
    [SerializeField] private InputManager playerInput;
    [SerializeField] private GunPositioner gunPositioner;
    [SerializeField] private SoftbodyHolder softbodyHolder;
    [SerializeField] private SpriteRenderer liquidRenderer;

    [Header("SoftBodies Prefabs List")]
    [SerializeField] private SoftBody[] softbodies;

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
        gunPositioner.SetCanShoot(true);
        suckedInShape = softbody.softbodyShape;
        SoftBodyForceApplier forceApplier = softbody.GetComponent<SoftBodyForceApplier>();
        softbodyHolder.softbodies.Remove(forceApplier.transform.GetChild(0));
        sucker.SuckIn(forceApplier, transform, suckInPower, liquidRenderer, OnSuckComplete);
        suckIneffect.ActivateSuckInEffect(softbody.transform.GetChild(0));
    }

    private void ShootSoftBody()
    {
        canShoot = false;
        gunPositioner.SetCanShoot(false);
        SoftBodyForceApplier newBall = Instantiate(GetSuckedInShape(), gunEnd.position, Quaternion.identity, cannonBallHolder).GetComponent<SoftBodyForceApplier>();
        softbodyHolder.softbodies.Add(newBall.transform.GetChild(0));
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

    private void OnDestroy()
    {
        playerInput.OnFire -= Shoot;
    }
}
