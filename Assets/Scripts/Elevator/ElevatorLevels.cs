using System.Collections;
using UnityEngine;

public class ElevatorLevels : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] levels;
    [SerializeField] private Transform arrow;
    [SerializeField] Color highlightedLevelColor;
    [SerializeField] Color currentLevelColor;
    [SerializeField] ElevatorManager elevatorManager; // Reference to ElevatorManager

    public float arrowSpeed = 1f; // Units per second for arrow movement
    private int currentLevel = 1; // Starting at floor 1
    private int targetLevel = 3;

    public void GoUp()
    {
        if (currentLevel >= targetLevel) return;
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
        if (level < 1 || level > targetLevel) return;
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

        levels[targetLevel - 1].color = highlightedLevelColor;
        levels[currentLevel - 1].color = currentLevelColor;
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetTargetLevel() => targetLevel;

    // This method is called every frame while the lever is being dragged.
    // normalizedInput should be in the range [-1, 1]:
    // +1 means lever is at +45° (full right -> elevator up),
    // -1 means lever is at -45° (full left -> elevator down).
    public void UpdateArrowMovement(float normalizedInput)
    {
        // Calculate how much to move the arrow this frame:
        float delta = arrowSpeed * normalizedInput * Time.deltaTime;
        float newX = arrow.position.x + delta;

        // Clamp arrow position between first and last level positions.
        float minX = levels[0].transform.position.x;
        float maxX = levels[levels.Length - 1].transform.position.x;
        newX = Mathf.Clamp(newX, minX, maxX);
        arrow.position = new Vector3(newX, arrow.position.y, arrow.position.z);

        // Check if the arrow is close enough to any level's position.
        float threshold = 0.05f; // Adjust as needed.
        for (int i = 0; i < levels.Length; i++)
        {
            if (Mathf.Abs(newX - levels[i].transform.position.x) < threshold)
            {
                int reachedFloor = i + 1;
                if (reachedFloor != currentLevel)
                {
                    currentLevel = reachedFloor;
                    GoTo(currentLevel);
                    // Trigger the mini game for the reached floor.
                    elevatorManager.StartMiniGameForFloor(currentLevel);
                }
            }
        }
    }
}