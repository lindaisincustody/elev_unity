using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MainEnemy : BaseEnemy
{
    [SerializeField] float rageModeDuration = 7f;
    [SerializeField] float rageModeCooldown = 15f;
    [SerializeField] private State.ENEMYTYPE enemyType;

    [Header("Body references")]
    [SerializeField] private GameObject eyes;
    [SerializeField] private GameObject[] children;

    [Header("Self refernces")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private ParticleSystemScaler particleManager;
    [SerializeField] private ParticleSystem[] particleSystems;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    private bool isActive = false;

    void Update()
    {
        if (!isActive)
            return;

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

    private void DeactivateMainEnemy()
    {
        if (gameObject == null && !gameObject.activeSelf)
            return;
        enemySpawner.ExitRageMode(transform.position);

        particleManager.SetParticleSizes(false, HideMainEnemyBody);
        isActive = false;
        navAgent.enabled = false;
        eyes.SetActive(false);
    }

    private void ActivateMainEnemy()
    {
        InitializeMainEnemy();
        Invoke("DeactivateMainEnemy", rageModeDuration);
    }

    private void InitializeMainEnemy()
    {
        Vector3? newPos = enemySpawner.ChoosePatrolEnemyToRageMode();
        if (newPos == null)
            return;

        transform.position = (Vector3)newPos;

        particleManager.SetParticleSizes(true, StartMainEnemyMovement);
        navAgent.enabled = true;
        currentState = new Idle(gameObject, agent, target.transform, mazeGenerator, patrolSpeed, followSpeed, enemyType);

        ShowHideBody(true);
    }

    private void ShowHideBody(bool isHidden)
    {
        foreach (var item in children)
        {
            item.SetActive(isHidden);
        }
    }

    private void StartMainEnemyMovement()
    {
        isActive = true;
        eyes.SetActive(true);
    }

    private void HideMainEnemyBody()
    {
        ShowHideBody(false);
        Invoke("ActivateMainEnemy", rageModeCooldown);
    }

    private void LookAt2D(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = targetRotation;
    }

    public override void ActivateEnemy()
    {
        particleManager.StoreOriginalValues(particleSystems);
        Invoke("ActivateMainEnemy", rageModeCooldown);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(Constants.SceneNames.MainScene);
        }
    }
}
