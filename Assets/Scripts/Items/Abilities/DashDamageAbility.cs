using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash Damage Ability", menuName = "Custom/Ability/DashDamageAbility")]
public class DashDamageAbility : Ability
{
    [SerializeField] private int damage;
    [Header("Detection Settings")]
    [SerializeField, Tooltip("Radius around the player to check for enemies")]
    private float hitRadius = 0.5f;
    [SerializeField, Tooltip("Which layers count as enemies")]
    private LayerMask enemyLayer;

    private Coroutine _detectionCoroutine;
    private HashSet<Collider2D> _hitThisDash;

    public override void Start()
    {
        PlayerMovement pm = Player.instance.Get<PlayerMovement>();
        pm.OnDash += BeginDetection;
        pm.OnDashEnd += EndDetection;
    }

    public override void End()
    {
        PlayerMovement pm = Player.instance.Get<PlayerMovement>();
        pm.OnDash -= BeginDetection;
        pm.OnDashEnd -= EndDetection;
    }

    private void BeginDetection()
    {
        _hitThisDash = new HashSet<Collider2D>();

        Player.instance.Get<PlayerVisuals>().EnableTrail();
        _detectionCoroutine = CoroutineRunner.Instance
            .StartCoroutine(DetectAndDamage());
    }

    private void EndDetection()
    {
        Player.instance.Get<PlayerVisuals>().DisableTrail();
        if (_detectionCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_detectionCoroutine);
            _detectionCoroutine = null;
        }
    }

    private IEnumerator DetectAndDamage()
    {
        var t = Player.instance.transform;

        while (true)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                t.position,
                hitRadius,
                enemyLayer
            );

            foreach (var col in hits)
            {
                if (_hitThisDash.Add(col))
                    ApplyDamage(col);
            }

            yield return null;
        }
    }

    private void ApplyDamage(Collider2D enemyCol)
    {
        var hp = enemyCol.GetComponent<EnemyHealth>();
        if (hp != null)
            hp.TakeDamage(damage);
    }
}
