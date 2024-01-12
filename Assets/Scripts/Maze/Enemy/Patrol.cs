using UnityEngine;
using UnityEngine.AI;

public class Patrol : State
{
    public Patrol(GameObject _npc, NavMeshAgent _agent, Transform _player, MazeGenerator _mazeGenerator, float _patrolSpeed, float _followSpeed) 
        : base(_npc, _agent, _player, _mazeGenerator, _patrolSpeed, _followSpeed)
    {
        name = STATE.PATROL;
        patrolSpeed = _patrolSpeed;
        followSpeed = _followSpeed;
        agent.speed = _patrolSpeed;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (agent.remainingDistance < 1)
        {
            agent.SetDestination(mazeGenerator.GetRandomCellPosition());
        }

        if (canReach())
        {
            nextState = new Follow(enemyNPC, agent, player, mazeGenerator, patrolSpeed, followSpeed);
            stage = EVENT.EXIT;
        }

        if (agent.path.corners.Length > 1)
            objectivePos = agent.path.corners[1];

        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
