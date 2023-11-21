using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject xPrefab;
    public GameObject oPrefab;
    public int gridSizeX = 3;
    public int gridSizeY = 3;

    private GameObject[,] grid;

    // Define a list of positions representing the path the player should take
    private Vector2Int[] path = { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2), new Vector2Int(2, 1) };

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new GameObject[gridSizeX, gridSizeY];

        for (int i = 0; i < path.Length; i++)
        {
            int x = path[i].x;
            int y = path[i].y;

            // Instantiate O prefabs at positions defined by the path
            GameObject obj = Instantiate(oPrefab, new Vector3(x, y, 0), Quaternion.identity);
            grid[x, y] = obj;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Adjust the condition as needed
        {
            CheckWinCondition();
        }
    }

    void CheckWinCondition()
    {
        bool allX = true;

        for (int i = 0; i < path.Length; i++)
        {
            int x = path[i].x;
            int y = path[i].y;

            if (grid[x, y].CompareTag("O"))
            {
                allX = false;
                // Player didn't win yet, some objects are still O
                Debug.Log("Object at position (" + x + ", " + y + ") is still O.");
                return;
            }
        }

        if (allX)
        {
            // Player won, all objects are X
            Debug.Log("You won! All objects turned into X.");
            // Implement win condition logic, e.g., display a win screen.
        }
    }
}