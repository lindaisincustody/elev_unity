using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Brain : MonoBehaviour
{
    [SerializeField] private Sensor sensor;

    public Action ActionReset { get; private set; }

    public List<ActionData> actions;
    public Context context;

    public Enemy enemy;

    private bool _isActionBusy;

    public bool isActionBusy
    {
        get
        {
            return _isActionBusy;
        }
        set
        {
            _isActionBusy = value;
        }
    }


    private void Awake()
    {
        context = new Context(this, sensor, enemy.Get<EnemyMovement>());

        foreach (var action in actions)
        {
            action.action.Initialize(context);
        }

        ActionReset += ActionsReset;
    }

    private void ActionsReset()
    {
        foreach (var action in actions)
        {
            action.action.Stop(context);
        }

        enemy.Get<EnemyAnimator>().StopAttack();
    }

    private void Update()
    {
        UpdateContext();

        if (isActionBusy || enemy.Get<Health>().isDead) return;

        enemy.Get<EnemyMovement>().Move();

        AIAction bestAction = null;
        float hightestUtility = float.MinValue;

        foreach (var action in actions)
        {
            if (!action.available)
                continue;
            float utility = action.action.CalculateUtility(context);
            if (utility > hightestUtility)
            {
                hightestUtility = utility;
                bestAction = action.action;
            }
        }

        if (bestAction != null)
        {
            bestAction.Execute(context);
        }

        foreach (var action in actions)
        {
            if (action.action != bestAction)
            {
                action.action.Reset(context);
            }
        }
    }

    public void SetActionState(ActionType actionType, bool state)
    {
        foreach (var item in actions)
        {
            if (item.actionType == actionType)
            {
                item.available = state;
            }
        }
    }

    private void UpdateContext()
    {
        context.SetData("Health", enemy.Get<EnemyHealth>().NormalizedHealth());
    }

    private void OnDestroy()
    {
        ActionReset -= ActionsReset;
    }

    [System.Serializable]
    public class ActionData
    {
        public AIAction action;
        public ActionType actionType;
        public bool available = true;
    }

    public enum ActionType
    {
        Idle,
        Roam,
        Attack,
        Phase,
        ChasePlayer,
        Dash,
        SpecialAttack
    }
}
