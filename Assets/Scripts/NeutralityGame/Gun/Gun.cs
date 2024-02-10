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
    [SerializeField] GameObject bullet;
    SoftBody.Shape suckedInShape;

    SoftbodySucker sucker = new SoftbodySucker();

    bool _canShoot = false;
    bool _canSuck = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canSuck)
            {
                SuckInSoftBody();
            }
            else if (_canShoot)
            {
                ShootSoftBody();
            }
        }
    }

    private void SuckInSoftBody()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, distance, consumableLayer);
        if (!hit.collider)
            return;

        SoftBody softbody = hit.transform.parent.gameObject.GetComponent<SoftBody>();
        suckedInShape = softbody.softbodyShape;
        SoftBodyForceApplier forceApplier = softbody.GetComponent<SoftBodyForceApplier>();
        sucker.SuckIn(forceApplier, transform, suckInPower, liquidRenderer, OnSuckComplete);
        suckIneffect.ActivateSuckInEffect(softbody.transform.GetChild(0));
    }

    private void ShootSoftBody()
    {
        SoftBodyForceApplier newBall = Instantiate(GetSuckedInShape(), transform.position, Quaternion.identity, cannonBallHolder).GetComponent<SoftBodyForceApplier>();
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
