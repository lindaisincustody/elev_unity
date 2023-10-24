using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] private int triggerDistance;
    [SerializeField] private float timerDuration;
    [SerializeField] private int travelTriggerDistance;
    [SerializeField] private Gradient startGradient;
    [SerializeField] private Gradient resetGradient;

    // Components & References
    private MazeGenerator mazeGenerator;
    private Transform target;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    private PathRenderer pathRenderer;
    private GlitchController fullscreenShader;

    // State Variables
    private bool canFollow = false;
    private bool timerRunning = false;
    private bool caught = false;
    private float currentTimer;
    private float distanceToTarget;
    private float pathLength;

    // Constants & Settings
    private const int fadeSteps = 60;
    private const float agentSpeed = 5f;
    private const float caughtSpeed = 10f;
    private Coroutine coroutine;

    private void Awake()
    {
        mazeGenerator = FindAnyObjectByType<MazeGenerator>();
        target = FindAnyObjectByType<MazePlayerMovement>().transform;
        lineRenderer = GetComponent<LineRenderer>();
        pathRenderer = GetComponent<PathRenderer>();

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
        mazeGenerator.OnMazeCompletion.AddListener(EnableEnemies);
    }

    private void OnDisable()
    {
        // Unsubscribe from the custom event
        mazeGenerator.OnMazeCompletion.RemoveListener(EnableEnemies);
    }

    private void EnableEnemies()
    {
        Invoke("FollowPlayer", 2);
    }

    private void FollowPlayer()
    {
        canFollow = true;
    }

    void Update()
    {
        if (!canFollow)
            return;
        distanceToTarget = Vector2.Distance(gameObject.transform.position, target.transform.position);

        if (distanceToTarget > triggerDistance)
        {
            if (!caught)
            {
                ResetTimer();
                return;
            }

        }
        agent.SetDestination(target.position);
        if (caught)
            return;
        if (isPathTooLong())
        {
            pathRenderer.drawLine = false;
            agent.speed = 0f;
            return;
        }
        else
        {
            pathRenderer.drawLine = true;
            agent.speed = agentSpeed;
        }
        // Player is within range, start the timer
        StartTimer();

        if (timerRunning)
        {
            // Update the timer
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                // Timer has reached 0, call your method here
                CallMethodWhenTimerEnds();

                // Reset the timer
                ResetTimer();
            }
        }
    }

    private bool isPathTooLong()
    {
        Vector3[] pathPoints = agent.path.corners;
        pathLength = CalculatePathLength(pathPoints);

        if (pathLength < travelTriggerDistance)
            return false;
        return true;
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

    private void StartTimer()
    {
        if (!timerRunning)
        {
            currentTimer = timerDuration;
            timerRunning = true;

            // Start the gradient fading coroutine
            coroutine = StartCoroutine(FadeGradient(startGradient, resetGradient, timerDuration));
        }
    }

    private void ResetTimer()
    {
        if (!timerRunning)
            return;

        timerRunning = false;
        currentTimer = 0f;

        if (coroutine != null)
            StopCoroutine(coroutine);
        if (!caught)
            lineRenderer.colorGradient = startGradient;
    }

    public static Gradient InterpolateGradients(Gradient startGradient, Gradient endGradient, float t)
    {
        Gradient interpolatedGradient = new Gradient();
        int numColorKeys = Mathf.Min(startGradient.colorKeys.Length, endGradient.colorKeys.Length);
        GradientColorKey[] interpolatedColorKeys = new GradientColorKey[numColorKeys];

        for (int i = 0; i < numColorKeys; i++)
        {
            Color interpolatedColor = Color.Lerp(startGradient.colorKeys[i].color, endGradient.colorKeys[i].color, t);
            float interpolatedTime = Mathf.Lerp(startGradient.colorKeys[i].time, endGradient.colorKeys[i].time, t);
            interpolatedColorKeys[i] = new GradientColorKey(interpolatedColor, interpolatedTime);
        }

        interpolatedGradient.colorKeys = interpolatedColorKeys;
        return interpolatedGradient;
    }


    private IEnumerator FadeGradient(Gradient startGradient, Gradient endGradient, float duration)
    {
        if (lineRenderer != null)
        {
            for (int step = 0; step < fadeSteps; step++)
            {
                float t = step / (float)fadeSteps;

                Gradient interpolatedGradient = InterpolateGradients(startGradient, endGradient, t);
                lineRenderer.colorGradient = interpolatedGradient;

                yield return new WaitForSeconds(duration / fadeSteps);
            }
            lineRenderer.colorGradient = endGradient;
        }
    }

    private void CallMethodWhenTimerEnds()
    {
        agent.speed = caughtSpeed;
        caught = true;
        StopCoroutine(coroutine);
        lineRenderer.colorGradient = resetGradient;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fullscreenShader.TriggerCaughtShader();
            mazeGenerator.DeactivateShortestPath();
            Destroy(gameObject);
        }
    }
}
