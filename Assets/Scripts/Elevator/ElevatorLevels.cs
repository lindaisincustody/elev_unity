using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorLevels : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] levels;
    [SerializeField] private Transform arrow;
    [SerializeField] Color highlightedLevelColor;
    [SerializeField] Color currentLevelColor;

    private float arrowSpeed = 1f;
    private int currentLevel = 1;
    private int targetLevel = 3;

    public void GoDown()
    {
        if (currentLevel - 1 < 0)
            return;

        currentLevel--;
        StartCoroutine(MoveArrow(levels[currentLevel - 1].transform.position));
    }

    public void GoUp()
    {
        if (currentLevel + 1 > levels.Length - 1)
            return;

        currentLevel++;
        StartCoroutine(MoveArrow(levels[currentLevel - 1].transform.position));
    }

    public void GoTo(int level)
    {
        if (level > levels.Length - 1 || level < 0)
            return;

        StartCoroutine(MoveArrow(levels[level].transform.position));
    }

    private IEnumerator MoveArrow(Vector3 moveTo)
    {
        float startX = arrow.position.x;
        float distance = Mathf.Abs(moveTo.x - startX);
        float totalTime = distance / arrowSpeed;
        float elapsedTime = 0;

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

        // minus one, because levels start from 1
        levels[targetLevel - 1].color = highlightedLevelColor;
        levels[currentLevel - 1].color = currentLevelColor;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }    

    public int GetTargetLevel()
    {
        return targetLevel;
    }
}
