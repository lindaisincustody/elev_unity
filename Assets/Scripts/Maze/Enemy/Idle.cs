using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    private float idleDuration = 4f;
    private float timeEnteredState;

    public Idle(GameObject _npc, NavMeshAgent _agent, Transform _player, MazeGenerator _mazeGenerator, float _patrolSpeed, float _followSpeed, ENEMYTYPE _enemyType) 
        : base(_npc, _agent, _player, _mazeGenerator, _patrolSpeed, _followSpeed, _enemyType)
    {
        patrolSpeed = _patrolSpeed;
        followSpeed = _followSpeed;
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        base.Enter();
        timeEnteredState = Time.time;
        if (enemyType == ENEMYTYPE.MAIN)
            idleDuration = 0.5f;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public override void Update()
    {
        if (Time.time - timeEnteredState > idleDuration)
        {
            switch (enemyType)
            {
                case ENEMYTYPE.PATROL:
                    nextState = new Patrol(enemyNPC, agent, player, mazeGenerator, patrolSpeed, followSpeed, enemyType);
                    break;
                case ENEMYTYPE.MAIN:
                    nextState = new Follow(enemyNPC, agent, player, mazeGenerator, patrolSpeed, followSpeed, enemyType);
                    break;
            };

            stage = EVENT.EXIT;
            return;
        }
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
