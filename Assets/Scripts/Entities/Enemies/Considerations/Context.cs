using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Context
{
    public Brain brain;
    public EnemyMovement movement;
    public Transform target;
    public Sensor sensor;

    readonly Dictionary<string, object> data = new();

    public Context(Brain brain, Sensor sensor, EnemyMovement movement)
    {
        this.brain = brain;
        this.movement = movement;
        this.sensor = sensor;
    }

    public T GetData<T>(string key) => data.TryGetValue(key, out var value) ? (T)value : default;
    public void SetData(string key, object value) => data[key] = value;
}
