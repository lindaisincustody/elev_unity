using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Brain : MonoBehaviour
{
    [SerializeField] private Sensor sensor;

    public List<AIAction> actions;
    public Context context;
    
    public EnemyMovement movement;
    public EnemyHealth health;
    public EnemyAttack attack;
    public EnemyAnimator animator;

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
        context = new Context(this, sensor, movement);
        health = GetComponent<EnemyHealth>();

        foreach (var action in actions)
        {
            action.Initialize(context);
        }
    }

    private void Update()
    {
        UpdateContext();

        if (isActionBusy) return;

        AIAction bestAction = null;
        float hightestUtility = float.MinValue;

        foreach (var action in actions)
        {
            float utility = action.CalculateUtility(context);
            if (utility > hightestUtility)
            {
                hightestUtility = utility;
                bestAction = action;
            }
        }

        if (bestAction != null)
        {
            bestAction.Execute(context);
        }

        foreach (var action in actions)
        {
            if (action != bestAction)
            {
                action.Reset(context);
            }
        }
    }

    private void UpdateContext()
    {
        context.SetData("Health", health.NormalizedHealth());
    }
}
