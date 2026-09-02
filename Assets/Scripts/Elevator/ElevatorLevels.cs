using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorLevels : MonoBehaviour
{
    [SerializeField] private Image[] levels;
    [SerializeField] private RectTransform arrow;
    [SerializeField] Color highlightedLevelColor;
    [SerializeField] Color currentLevelColor;
    [SerializeField] ElevatorManager elevatorManager;

    public float arrowSpeed = 75f;
    public float snapThreshold = 4f;

    private int currentLevel = 1;
    private int targetLevel = 1;

    public void SetTargetLevel(int level)
    {
        if (level < 1 || level > levels.Length) return;
        targetLevel = level;
        HighlightCurrentLevel();
    }

    public void ResetToGround()
    {
        StopAllCoroutines();
        currentLevel = 1;
        targetLevel = 1;
        SetArrowX(LevelX(1));
        HighlightCurrentLevel();
    }

    public void GoUp()
    {
        if (currentLevel >= levels.Length) return;
        currentLevel++;
        StartCoroutine(MoveArrow(LevelX(currentLevel)));
    }

    public void GoDown()
    {
        if (currentLevel <= 1) return;
        currentLevel--;
        StartCoroutine(MoveArrow(LevelX(currentLevel)));
    }

    public void GoTo(int level)
    {
        if (level < 1 || level > levels.Length) return;
        currentLevel = level;
        StartCoroutine(MoveArrow(LevelX(level)));
    }

    private IEnumerator MoveArrow(float moveToX)
    {
        float startX = arrow.anchoredPosition.x;
        float distance = Mathf.Abs(moveToX - startX);
        float totalTime = distance / arrowSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            SetArrowX(Mathf.Lerp(startX, moveToX, elapsedTime / totalTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetArrowX(moveToX);
        HighlightCurrentLevel();
    }

    public void HighlightCurrentLevel()
    {
        foreach (Image item in levels)
        {
            item.color = Color.white;
        }

        levels[targetLevel - 1].color = highlightedLevelColor;
        levels[currentLevel - 1].color = currentLevelColor;
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetTargetLevel() => targetLevel;

    public void UpdateArrowMovement(float normalizedInput)
    {
        float delta = arrowSpeed * normalizedInput * Time.deltaTime;
        float minX = LevelX(1);
        float maxX = LevelX(levels.Length);
        float newX = Mathf.Clamp(arrow.anchoredPosition.x + delta, minX, maxX);

        SetArrowX(newX);

        for (int i = 0; i < levels.Length; i++)
        {
            if (Mathf.Abs(newX - LevelX(i + 1)) >= snapThreshold)
                continue;

            int reachedFloor = i + 1;
            if (reachedFloor == currentLevel)
                continue;

            currentLevel = reachedFloor;
            GoTo(currentLevel);
            elevatorManager.OnFloorReached(currentLevel);
        }
    }

    private float LevelX(int level)
    {
        return ((RectTransform)levels[level - 1].transform).anchoredPosition.x;
    }

    private void SetArrowX(float x)
    {
        arrow.anchoredPosition = new Vector2(x, arrow.anchoredPosition.y);
    }
}
