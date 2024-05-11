using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class PatrolEnemy : BaseEnemy
{
    [SerializeField] private float respawnTime = 2f;
    [SerializeField] private State.ENEMYTYPE enemyType;

    [Header("Body references")]
    [SerializeField] private GameObject eyes;
    [SerializeField] private GameObject[] children;

    [Header("Self refernces")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private ParticleSystemScaler particleManager;
    [SerializeField] private GameObject deathPS;
    [SerializeField] private ParticleSystem[] particleSystems;

    private bool isHidden = false;
    private Vector3 spawnPos;

    void Update()
    {
        if (currentState != null)
        {
            State nextState = currentState.Process();

            if (nextState != null)
            {
                currentState = nextState;
            }

            if (currentState.name != State.STATE.IDLE)
                LookAt2D(currentState.GetObjectivePosition());
        }
    }

    private void LookAt2D(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = targetRotation;
    }

    public void HideEnemy(bool showAnimation)
    {
        isHidden = true;
        if (showAnimation)
            particleManager.SetParticleSizes(false, HideEnemyBody);
        else
        {
            HideEnemyBody();
            StartCoroutine(RespawnEnemy());
        }
        eyes.SetActive(false);
        navAgent.enabled = false;
        currentState = null;
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(respawnTime);
        AppearEnemy(spawnPos);
    }

    public void AppearEnemy(Vector3 newPos)
    {
        transform.position = newPos;
        particleManager.SetParticleSizes(true, StartMoving);
        navAgent.enabled = true;
        foreach (var item in children)
        {
            item.SetActive(true);
        }
    }

    private void HideEnemyBody()
    {
        foreach (var item in children)
        {
            item.SetActive(false);
        }
    }

    private void StartMoving()
    {
        isHidden = false;
        eyes.SetActive(true);
        currentState = new Idle(gameObject, agent, target.transform, mazeGenerator, patrolSpeed, followSpeed, enemyType);
    }

    public override void ActivateEnemy()
    {
        SoundManager.PlaySound3D(SoundManager.Sound.Pulsating_1, transform, true, 0.1f, 1f, 40, 1f, 1.5f);
        spawnPos = transform.position;
        particleManager.StoreOriginalValues(particleSystems);
        currentState = new Idle(gameObject, agent, target.transform, mazeGenerator, patrolSpeed, followSpeed, enemyType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isHidden)
        {
            fullscreenShader.TriggerCaughtShader();
            mazeGenerator.DeactivateShortestPath();
            effectManager.ActivateDissolveWallsEffect();
            Instantiate(deathPS, transform.position, Quaternion.identity);
            HideEnemy(false);
        }
    }
}
