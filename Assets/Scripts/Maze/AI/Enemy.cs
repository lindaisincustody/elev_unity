using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    MazeGenerator mazeGenerator;
    Transform target;
    NavMeshAgent agent;

    private bool canFollow = false;
    [SerializeField] private int triggerDistance;
    [SerializeField] private float timerDuration; // Duration of the timer in seconds
    [SerializeField] private int travelTriggerDistance;
    private LineRenderer lineRenderer;
    [SerializeField] private Gradient startGradient; // Gradient when timer starts
    [SerializeField] private Gradient resetGradient; // Gradient when timer resets


    private float currentTimer; // Current time on the timer
    private bool timerRunning = false; // Flag to check if the timer is running

    private bool isFadingGradient = false; // Flag to track whether gradient fading is in progress
    private float fadeDuration = 2.0f;
    private int fadeSteps = 60;

    float distanceToTarget;
    Coroutine coroutine;
    private float pathLength;

    private float agentSpeed = 5;
    private bool caught = false;
    PathRenderer pathRenderer;

    private void Awake()
    {
        mazeGenerator = FindAnyObjectByType<MazeGenerator>();
        target = FindAnyObjectByType<MazePlayerMovement>().transform;
        lineRenderer = GetComponent<LineRenderer>();
        pathRenderer = GetComponent<PathRenderer>();
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

    // Update is called once per frame
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
            coroutine = StartCoroutine(FadeGradient(startGradient, resetGradient, fadeDuration));
        }
    }

    private void ResetTimer()
    {
        timerRunning = false;
        currentTimer = 0f;

        if (coroutine != null)
        StopCoroutine(coroutine);
        lineRenderer.colorGradient = startGradient;
    }

    public static Gradient InterpolateGradients(Gradient startGradient, Gradient endGradient, float t)
    {
        // Create a new gradient
        Gradient interpolatedGradient = new Gradient();

        // Determine the number of color keys to interpolate (use the smaller count)
        int numColorKeys = Mathf.Min(startGradient.colorKeys.Length, endGradient.colorKeys.Length);

        // Create arrays to hold the interpolated color keys
        GradientColorKey[] interpolatedColorKeys = new GradientColorKey[numColorKeys];

        for (int i = 0; i < numColorKeys; i++)
        {
            GradientColorKey startColorKey = startGradient.colorKeys[i];
            GradientColorKey endColorKey = endGradient.colorKeys[i];

            // Interpolate the color
            Color interpolatedColor = Color.Lerp(startColorKey.color, endColorKey.color, t);

            // Interpolate the time value
            float interpolatedTime = Mathf.Lerp(startColorKey.time, endColorKey.time, t);

            // Create the interpolated color key
            interpolatedColorKeys[i] = new GradientColorKey(interpolatedColor, interpolatedTime);
        }

        // Set the color keys of the interpolated gradient
        interpolatedGradient.colorKeys = interpolatedColorKeys;

        // Return the interpolated gradient
        return interpolatedGradient;
    }


    private IEnumerator FadeGradient(Gradient startGradient, Gradient endGradient, float duration)
    {
        if (lineRenderer != null)
        {
            float elapsed = 0f;

            for (int step = 0; step < fadeSteps; step++)
            {
                float t = step / (float)fadeSteps;

                // Interpolate the gradients
                Gradient interpolatedGradient = InterpolateGradients(startGradient, endGradient, t);
                lineRenderer.colorGradient = interpolatedGradient;

                // Wait for a fraction of the duration
                yield return new WaitForSeconds(duration / fadeSteps);
            }

            // Ensure the gradient is set to the target gradient when the fading is complete
            lineRenderer.colorGradient = endGradient;
        }

        isFadingGradient = false;
    }

    private void CallMethodWhenTimerEnds()
    {
        Debug.Log("Caught");
        caught = true;
        StopCoroutine(coroutine);
        lineRenderer.colorGradient = resetGradient;
    }
}
