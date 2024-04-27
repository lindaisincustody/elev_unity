using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _unvisitedBlock;
    [SerializeField] private GameObject _shortestPathBlock;

    public bool isVisited { get; private set; }

    public void Visit()
    {
        isVisited = true;
        //_unvisitedBlock.SetActive(false);
    }

    public void ClearLeft()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRight()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFront()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBack()
    {
        _backWall.SetActive(false);
    }

    public void MarkAsExit(int x, int y, MazeCell[,] mazeGrid)
    {
        _leftWall.SetActive(false);
        _rightWall.SetActive(false);
        _frontWall.SetActive(false);
        _backWall.SetActive(false);

        mazeGrid[x - 1, y - 1]._backWall.SetActive(false);
        mazeGrid[x - 1, y - 3]._frontWall.SetActive(false);
        mazeGrid[x - 2, y - 2]._rightWall.SetActive(false);
    }

    public void DeleteRandomWall(int x, int y, MazeCell[,] mazeGrid)
    {
        // Create an array of walls to choose from
        GameObject[] Allwalls = { _leftWall, _rightWall, _frontWall, _backWall };
        List<GameObject> activeWalls = new List<GameObject>();

        foreach (GameObject wall in Allwalls)
        {
            if (wall.activeSelf)
            {
                activeWalls.Add(wall);
            }
        }

        // Convert the list to an array if necessary
        GameObject[] walls = activeWalls.ToArray();
        // Shuffle the array randomly
        ShuffleArray(walls);
        // Disable the first wall in the shuffled array (randomly chosen)
        if (walls.Length > 0)
        {
            walls[0].SetActive(false);
            // Determine which neighbor's wall to disable based on the chosen wall
            if (walls[0] == _leftWall)
            {
                // Disable the right wall of the left neighbor
                if (mazeGrid[x - 1, y] != null)
                    mazeGrid[x - 1, y]._rightWall.SetActive(false);
            }
            else if (walls[0] == _rightWall)
            {
                // Disable the left wall of the right neighbor
                if (mazeGrid[x + 1, y] != null)
                    mazeGrid[x + 1, y]._leftWall.SetActive(false);
            }
            else if (walls[0] == _frontWall)
            {
                // Disable the back wall of the front neighbor
                if (mazeGrid[x, y + 1] != null)
                    mazeGrid[x, y + 1]._backWall.SetActive(false);
            }
            else if (walls[0] == _backWall) 
            {
                // Disable the front wall of the back neighbor
                if (mazeGrid[x, y - 1] != null)
                    mazeGrid[x, y - 1]._frontWall.SetActive(false);
            }
        }
    }

    private void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public bool HasRightWall()
    {
        if (_rightWall.activeSelf)
            return true;
        return false;
    }

    public bool HasLeftWall()
    {
        if (_leftWall.activeSelf)
            return true;
        return false;
    }

    public bool HasFrontWall()
    {
        if (_frontWall.activeSelf)
            return true;
        return false;
    }

    public bool HasBackWall()
    {
        if (_backWall.activeSelf)
            return true;
        return false;
    }

    public void SetAsShortestPath()
    {
        _shortestPathBlock.SetActive(true);
    }

    public void SetDirection(Sprite newSprite)
    {
        _shortestPathBlock.GetComponentInChildren<SpriteRenderer>().sprite = newSprite;
    }

    public void EnableShortedBlock()
    {
        _shortestPathBlock.SetActive(true);
    }
    public void DisableShortedBlock()
    {
        _shortestPathBlock.SetActive(false);

    }
}