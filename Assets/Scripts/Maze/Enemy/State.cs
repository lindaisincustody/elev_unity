using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE
    {
        IDLE, PATROL, FOLLOW
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public enum ENEMYTYPE
    {
        PATROL, EXPANDING, MAIN
    };

    public STATE name;
    protected EVENT stage;
    protected ENEMYTYPE enemyType;
    protected GameObject enemyNPC;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent;
    protected MazeGenerator mazeGenerator;
    protected Vector3 objectivePos;

    protected float patrolSpeed = 0;
    protected float followSpeed = 0;

    public State(GameObject _npc, NavMeshAgent _agent, Transform _player, MazeGenerator _mazeGenerator, float _patrolSpeed, float _followSpeed, ENEMYTYPE _enemyType)
    {
        enemyNPC = _npc;
        agent = _agent;
        player = _player;
        mazeGenerator = _mazeGenerator;
        stage = EVENT.ENTER;
        patrolSpeed = _patrolSpeed;
        followSpeed = _followSpeed;
        enemyType = _enemyType;
    }

    public virtual void Enter() {
        stage = EVENT.UPDATE; 
    }
    public virtual void Update() { }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public Vector3 GetObjectivePosition()
    {
        if (objectivePos == Vector3.zero && name != STATE.IDLE)
            return player.position;
        return objectivePos;
    }

    public bool canReach()
    {
        return false;
    }


    private float CalculatePathLength(Vector3[] pathPoints)
    {
        float length = 0f;

        if (pathPoints.Length < 2)
            return length;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            length += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }

        return length;
    }
}
