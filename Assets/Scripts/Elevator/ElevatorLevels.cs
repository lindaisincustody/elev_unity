using System.Collections;
using UnityEngine;

public class ElevatorLevels : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] levels;
    [SerializeField] private Transform arrow;
    [SerializeField] Color highlightedLevelColor;
    [SerializeField] Color currentLevelColor;
    [SerializeField] ElevatorManager elevatorManager;

    public float arrowSpeed = 1f;
    private int currentLevel = 1;
    private int targetLevel = 1;

    public void SetTargetLevel(int level)
    {
        if (level < 1 || level > levels.Length) return;
        targetLevel = level;
        HighlightCurrentLevel();
    }

    public void GoUp()
    {
        if (currentLevel >= levels.Length) return;
        currentLevel++;
        StartCoroutine(MoveArrow(levels[currentLevel - 1].transform.position));
    }

    public void GoDown()
    {
        if (currentLevel <= 1) return;
        currentLevel--;
        StartCoroutine(MoveArrow(levels[currentLevel - 1].transform.position));
    }

    public void GoTo(int level)
    {
        if (level < 1 || level > levels.Length) return;
        currentLevel = level;
        StartCoroutine(MoveArrow(levels[level - 1].transform.position));
    }

    private IEnumerator MoveArrow(Vector3 moveTo)
    {
        float startX = arrow.position.x;
        float distance = Mathf.Abs(moveTo.x - startX);
        float totalTime = distance / arrowSpeed;
        float elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;
            float newX = Mathf.Lerp(startX, moveTo.x, t);
            arrow.position = new Vector3(newX, arrow.position.y, arrow.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        arrow.position = new Vector3(moveTo.x, arrow.position.y, arrow.position.z);
        HighlightCurrentLevel();
    }

    public void HighlightCurrentLevel()
    {
        foreach (var item in levels)
        {
            item.color = Color.white;
        }

        if (targetLevel - 1 < levels.Length)
            levels[targetLevel - 1].color = highlightedLevelColor;
        if (currentLevel - 1 < levels.Length)
            levels[currentLevel - 1].color = currentLevelColor;
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetTargetLevel() => targetLevel;

    public void UpdateArrowMovement(float normalizedInput)
    {
        float delta = arrowSpeed * normalizedInput * Time.deltaTime;
        float newX = arrow.position.x + delta;

        float minX = levels[0].transform.position.x;
        float maxX = levels[levels.Length - 1].transform.position.x;
        newX = Mathf.Clamp(newX, minX, maxX);
        arrow.position = new Vector3(newX, arrow.position.y, arrow.position.z);

        float threshold = 0.05f;
        for (int i = 0; i < levels.Length; i++)
        {
            if (Mathf.Abs(newX - levels[i].transform.position.x) < threshold)
            {
                int reachedFloor = i + 1;
                if (reachedFloor != currentLevel)
                {
                    currentLevel = reachedFloor;

                    GoTo(currentLevel);
                    elevatorManager.StartMiniGameForFloor(currentLevel);
                }
            }
        }
    }
}