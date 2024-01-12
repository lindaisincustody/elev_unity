using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
    [SerializeField] private GameObject deathPS;
    void Update()
    {
        if (currentState != null)
        {
            State nextState = currentState.Process();

            if (nextState != null)
            {
                currentState = nextState;
            }
        }

        if (currentState.name != State.STATE.IDLE)
            LookAt2D(currentState.GetObjectivePosition());
    }

    private void LookAt2D(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = targetRotation;
    }

    public override void ActivateEnemy()
    {
        currentState = new Idle(gameObject, agent, target.transform, mazeGenerator, patrolSpeed, followSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fullscreenShader.TriggerCaughtShader();
            mazeGenerator.DeactivateShortestPath();
            effectManager.ActivateDissolveWallsEffect();
            Instantiate(deathPS, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
