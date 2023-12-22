using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image timer;
    [SerializeField] private Image crackedTimer;
    [SerializeField] private MazeGenerator mazeGenerator;
    [SerializeField] private RectTransform particleSystemHolder;

    private float xMax = 142f; private float xMin = -142f;

    public float totalTime;
    private float currentTime;
    private float hitTimePenaltyPerc = 0.2f; 

    private bool isCountingDown = false;

    public float catchUpSpeed = 1.0f;

    private void Start()
    {
        currentTime = totalTime;
    }

    public void StartCountdown()
    {
        isCountingDown = true;
        currentTime = totalTime;
    }

    void Update()
    {
        if (isCountingDown)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                if (crackedTimer.fillAmount <= 0)
                    isCountingDown = false;
            }

            timer.fillAmount = currentTime / totalTime;
            float newXPosition = Mathf.Lerp(xMin, xMax, timer.fillAmount);

            Vector3 newPosition = particleSystemHolder.localPosition;
            newPosition.x = newXPosition;
            particleSystemHolder.localPosition = newPosition;

            // Calculate the difference between the two fill amounts
            float difference = Mathf.Abs(timer.fillAmount - crackedTimer.fillAmount);

            // Adjust the speed based on the difference
            float adjustedSpeed = catchUpSpeed + difference * catchUpSpeed * 12;

            // Update the crackedTimer's fill amount
            crackedTimer.fillAmount = Mathf.MoveTowards(crackedTimer.fillAmount, timer.fillAmount, adjustedSpeed * Time.deltaTime);

        }
    }

    public void PlayerGotHit()
    {
        currentTime -= totalTime * hitTimePenaltyPerc;
    }

    private void OnEnable()
    {
        mazeGenerator.OnMazeCompletion.AddListener(StartCountdown);
    }

    private void OnDisable()
    {
        mazeGenerator.OnMazeCompletion.RemoveListener(StartCountdown);
    }

}
