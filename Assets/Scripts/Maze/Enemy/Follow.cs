using UnityEngine;
using UnityEngine.AI;

public class Follow : State
{
    public Follow(GameObject _npc, NavMeshAgent _agent, Transform _player, MazeGenerator _mazeGenerator, float _patrolSpeed, float _followSpeed) 
        : base(_npc, _agent, _player, _mazeGenerator, _patrolSpeed, _followSpeed)
    {
        name = STATE.FOLLOW;
        patrolSpeed = _patrolSpeed;
        followSpeed = _followSpeed;
        agent.speed = followSpeed;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(player.position);

        if (!agent.hasPath)
            return;

        if (!canReach())
        {
            nextState = new Idle(enemyNPC, agent, player, mazeGenerator, patrolSpeed, followSpeed);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
