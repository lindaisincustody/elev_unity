using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public float patrolSpeed;
    public float followSpeed;

    // Components & References
    protected MazeGenerator mazeGenerator;
    protected Transform target;
    protected NavMeshAgent agent;
    protected GlitchController fullscreenShader;
    protected EffectManager effectManager;
    protected State currentState;

    private void Awake()
    {
        mazeGenerator = FindAnyObjectByType<MazeGenerator>();
        target = FindAnyObjectByType<MazePlayerMovement>().transform;
        effectManager = FindObjectOfType<EffectManager>();
        fullscreenShader = FindObjectOfType<GlitchController>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        mazeGenerator.OnMazeCompletion.AddListener(ActivateEnemy);
    }

    private void OnDisable()
    {
        mazeGenerator.OnMazeCompletion.RemoveListener(ActivateEnemy);
    }

    public virtual void ActivateEnemy()
    {
        throw new NotImplementedException("Please override ActivateEnemy in child class");
    }
}