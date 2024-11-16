using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Component : MonoBehaviour
{
    protected Enemy Enemy;

    public void Init(Enemy enemy)
    {
        Enemy = enemy;
    }
}
