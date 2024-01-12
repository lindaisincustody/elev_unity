using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
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
            Destroy(gameObject);
        }
    }
}
