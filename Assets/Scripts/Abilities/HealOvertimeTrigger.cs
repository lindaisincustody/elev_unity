using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOvertimeTrigger : MonoBehaviour
{
    public LayerMask LayerMask { get; set; }
    public float duration { get; set; } = 5f;
    public int heal { get; set; } = 1;
    public float interval { get; set; } = 0.5f;

    [Header("Follow Settings")]
    public Transform followTarget;
    public Vector3 followOffset = new Vector3(0, -1.25f, 0); 
    public float frequency = 5f;
    [Range(0f, 2f)]
    public float dampingRatio = 1f;

    private Vector3 followVelocity = Vector3.zero;

    private Dictionary<Entity, HealOvertimeEffect> activeEffects = new();

    private void Awake()
    {
        if (followTarget == null)
            followTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void FixedUpdate()
    {
        if (followTarget != null)
            FollowWithSecondOrder();
    }

    private void FollowWithSecondOrder()
    {
        float dt = Time.deltaTime;
   
        Vector3 desired = followTarget.position + followOffset;
        Vector3 toTarget = desired - transform.position;

        // a = ω²·x − 2·ζ·ω·v
        float ω = frequency;
        Vector3 accel = ω * ω * toTarget
                        - 2f * dampingRatio * ω * followVelocity;

        followVelocity += accel * dt;
        transform.position += followVelocity * dt;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & LayerMask.value) != 0 && collision.isTrigger)
        {
            Entity entity = collision.GetComponent<Entity>();
            HealOvertimeEffect effect = new HealOvertimeEffect(entity, heal, interval);
            entity.Get<EntityEffects>().Add(effect);
            activeEffects.Add(entity, effect);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & LayerMask.value) != 0 && collision.isTrigger)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (activeEffects.TryGetValue(entity, out HealOvertimeEffect effect))
            {
                entity.Get<EntityEffects>().Remove(effect);
                activeEffects.Remove(entity);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}