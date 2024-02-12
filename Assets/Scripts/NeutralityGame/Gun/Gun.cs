using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float distance = 10f;
    [SerializeField] int suckInPower;
    [SerializeField] int shootPower;
    [SerializeField] Transform cannonBallHolder;
    [SerializeField] LayerMask consumableLayer;
    [SerializeField] SpriteRenderer liquidRenderer;
    [SerializeField] SoftBody[] softbodies;
    [SerializeField] SuckInEffect suckIneffect;
    [SerializeField] Transform gunEnd;
    SoftBody.Shape suckedInShape;

    SoftbodySucker sucker = new SoftbodySucker();

    bool _canShoot = false;
    bool _canSuck = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot)
            {
                ShootSoftBody();
            }
            else if (_canSuck)
            {
                ShootWindStream();
            }
        }
    }

    private void ShootWindStream()
    {
        suckIneffect.ActivateWind();
    }

    public void SuckInSoftBody(SoftBody softbody)
    {
        Debug.Log(softbody.name);
        if (_canShoot)
            return;

        suckedInShape = softbody.softbodyShape;
        SoftBodyForceApplier forceApplier = softbody.GetComponent<SoftBodyForceApplier>();
        sucker.SuckIn(forceApplier, transform, suckInPower, liquidRenderer, OnSuckComplete);
        suckIneffect.ActivateSuckInEffect(softbody.transform.GetChild(0));
    }

    private void ShootSoftBody()
    {
        SoftBodyForceApplier newBall = Instantiate(GetSuckedInShape(), gunEnd.position, Quaternion.identity, cannonBallHolder).GetComponent<SoftBodyForceApplier>();
        newBall.GetComponent<ColorAssigner>().AssignBrightColor(sucker.GetColor());
        sucker.ShootSoftBody(newBall, shootPower, transform.right);
        liquidRenderer.color = Color.white;
        OnShootComplete();
    }

    private SoftBody GetSuckedInShape()
    {
        return softbodies[(int)suckedInShape];
    }

    private void OnSuckComplete()
    {
        _canShoot = true;
        _canSuck = false;
    }

    private void OnShootComplete()
    {
        _canShoot = false;
        _canSuck = true;
    }
}
