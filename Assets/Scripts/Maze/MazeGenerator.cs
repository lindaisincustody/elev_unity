using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private CameraTransition cameraTransition;
    [SerializeField] private GameObject endPrefab;

    [Header("Data Holder")]
    [SerializeField] private MazeDataSO mazeData;

    [Header("Maze Parameters")]
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private float mazeSpeed = 0.02f;

    [Header("Loops Parameters")]
    [SerializeField] private bool allowLoops = false;
    [SerializeField] private int numberOfLoops = 5; // Set the number of loops you want

    private int _mazeWidth;
    private int _mazeHeight;
    private MazeCell[,] mazeGrid;
    private int cellSize = 4;
    private List<MazeCell> shortestPath;

    private ImageHolder imageHolder;

    private void Awake()
    {
        imageHolder = GetComponent<ImageHolder>();
        _mazeWidth = mazeData.MazeWidth;
        _mazeHeight = mazeData.MazeHeight;
    }

    private IEnumerator Start()
    {
        cameraTransition.SetCamera(_mazeWidth, _mazeHeight, cellSize);
        mazeGrid = new MazeCell[_mazeWidth, _mazeHeight];
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                MazeCell newCell = Instantiate(_mazeCellPrefab, new Vector2(i * cellSize, j * cellSize), Quaternion.identity);
                mazeGrid[i, j] = newCell;
                newCell.transform.parent = gameObject.transform;
            }
        }

        // Set the exit cell
        MazeCell exitCell = mazeGrid[_mazeWidth - 1, _mazeHeight - 2];
        Instantiate(endPrefab, new Vector2(exitCell.transform.position.x + cellSize, exitCell.transform.position.y), Quaternion.identity);

        yield return GenerateMaze(null, mazeGrid[0, 0], exitCell); // Pass the exit cell

        CalculateShortestPath(mazeGrid[0, 0], exitCell);
        // Randomly delete walls to create loops only once
        if (allowLoops)
        {
            CreateLoops();
        }
        cameraTransition.StartZoomIn();
    }


    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell, MazeCell exitCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(mazeSpeed);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell, exitCell);
            }
        } while (nextCell != null);

        // Check if the current cell is the exit cell, and mark it as the exit
        if (currentCell == exitCell)
        {
            currentCell.MarkAsExit();
        }
    }



    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();

    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x / cellSize;
        int y = (int)currentCell.transform.position.y / cellSize;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = mazeGrid[x + 1, y];
            if (cellToRight.isVisited == false)
                yield return cellToRight;
        }

        if (x - 1  >= 0)
        {
            var cellToLeft = mazeGrid[x - 1, y];
            if (!cellToLeft.isVisited)
                yield return cellToLeft;
        }

        if (y + 1 < _mazeHeight)
        {
            var CellToFront = mazeGrid[x, y + 1];
            if (!CellToFront.isVisited)
                yield return CellToFront;
        }

        if (y - 1 >= 0)
        {
            var CellToBack = mazeGrid[x, y - 1];
            if (!CellToBack.isVisited)
                yield return CellToBack;
        }    
    }

    private void CreateLoops()
    {
        int loopsCreated = 0;

        while (loopsCreated < numberOfLoops)
        {
            int randomX = Random.Range(1, _mazeWidth - 1);
            int randomY = Random.Range(1, _mazeHeight - 1);

            var randomCell = mazeGrid[randomX, randomY];
            //Debug.Log(mazeGrid[randomX, randomY]);
            // Check if the cell is not on the edge of the maze
            if (randomCell != null)
            {
                // Implement logic to delete a wall in a way that creates a loop
                // For example, you can choose a random wall within the cell and delete it
                // Be sure to update your MazeCell class to handle wall deletion
                randomCell.DeleteRandomWall(randomX, randomY, mazeGrid);
                loopsCreated++;
            }
        }
    }

    private void CalculateShortestPath(MazeCell startCell, MazeCell exitCell)
    {
        Queue<MazeCell> queue = new Queue<MazeCell>();
        Dictionary<MazeCell, MazeCell> parentMap = new Dictionary<MazeCell, MazeCell>();
        HashSet<MazeCell> visited = new HashSet<MazeCell>();

        queue.Enqueue(startCell);
        parentMap[startCell] = null;

        while (queue.Count > 0)
        {
            MazeCell currentCell = queue.Dequeue();

            if (currentCell == exitCell)
                break;

            visited.Add(currentCell);

            foreach (var neighbor in GetNeighborsWithNoWalls(currentCell))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    parentMap[neighbor] = currentCell;
                }
            }
        }

        shortestPath = new List<MazeCell>();
        MazeCell cell = exitCell;
        MazeCell prevCell = null;

        while (cell != null)
        {
            shortestPath.Add(cell);
            cell.SetAsShortestPath();

            if (parentMap.ContainsKey(cell))
            {
                prevCell = cell;
                cell = parentMap[cell];
            }
            else
            {
                break;
            }
        }

        shortestPath.Reverse();
        foreach (var cells in shortestPath)
        {
            SetSpriteDirection(cells);
        }

    }

    private void SetSpriteDirection(MazeCell currentCell)
    {
        int currentIndex = shortestPath.IndexOf(currentCell);

        if (currentIndex > 0 && currentIndex < shortestPath.Count - 1)
        {
            MazeCell prevCell = shortestPath[currentIndex - 1];
            MazeCell nextCell = shortestPath[currentIndex + 1];

            // Determine the direction based on the relative positions of the cells
            if (prevCell.transform.position.x < currentCell.transform.position.x && nextCell.transform.position.x > currentCell.transform.position.x)
            {
                currentCell.SetDirection(imageHolder.Right);
            }
            else if (prevCell.transform.position.x > currentCell.transform.position.x && nextCell.transform.position.x < currentCell.transform.position.x)
            {
                currentCell.SetDirection(imageHolder.Left);
            }
            else if (prevCell.transform.position.y < currentCell.transform.position.y && nextCell.transform.position.y > currentCell.transform.position.y)
            {
                currentCell.SetDirection(imageHolder.Up);
            }
            else if (prevCell.transform.position.y > currentCell.transform.position.y && nextCell.transform.position.y < currentCell.transform.position.y)
            {
                currentCell.SetDirection(imageHolder.Down);
            }
            // Check for corner cases
            else if (prevCell.transform.position.x > currentCell.transform.position.x && nextCell.transform.position.y > currentCell.transform.position.y)
            {
                currentCell.SetDirection(imageHolder.RightUp);
            }
            else if (prevCell.transform.position.x < currentCell.transform.position.x && nextCell.transform.position.y > currentCell.transform.position.y)
            {
                currentCell.SetDirection(imageHolder.LeftUp);
            }
            else if (prevCell.transform.position.x < currentCell.transform.position.x && nextCell.transform.position.y < currentCell.transform.position.y)
            {
                currentCell.SetDirection(imageHolder.LeftDown);
            }
            else if (prevCell.transform.position.x > currentCell.transform.position.x && nextCell.transform.position.y < currentCell.transform.position.y)
            {
                currentCell.SetDirection(imageHolder.RightDown);
            }
            // Additional 4 corner cases
            else if (prevCell.transform.position.y < currentCell.transform.position.y && nextCell.transform.position.x > currentCell.transform.position.x)
            {
                currentCell.SetDirection(imageHolder.DownRight);
            }
            else if (prevCell.transform.position.y < currentCell.transform.position.y && nextCell.transform.position.x < currentCell.transform.position.x)
            {
                currentCell.SetDirection(imageHolder.DownLeft);
            }
            else if (prevCell.transform.position.y > currentCell.transform.position.y && nextCell.transform.position.x > currentCell.transform.position.x)
            {
                currentCell.SetDirection(imageHolder.UpRight);
            }
            else if (prevCell.transform.position.y > currentCell.transform.position.y && nextCell.transform.position.x < currentCell.transform.position.x)
            {
                currentCell.SetDirection(imageHolder.UpLeft);
            }
        }
        else
        {
            // Handle cases where there are no previous or next cells (start or end of path)
            // Set a default direction or use logic based on your requirements
            // For example:
            currentCell.SetDirection(imageHolder.Right);
        }
    }

    private IEnumerable<MazeCell> GetNeighborsWithNoWalls(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x / cellSize;
        int y = (int)currentCell.transform.position.y / cellSize;

        // Check the right neighbor
        if (x + 1 < _mazeWidth)
        {
            var cellToRight = mazeGrid[x + 1, y];
            if (!currentCell.HasRightWall() || !cellToRight.HasLeftWall())
                yield return cellToRight;
        }

        // Check the left neighbor
        if (x - 1 >= 0)
        {
            var cellToLeft = mazeGrid[x - 1, y];
            if (!currentCell.HasLeftWall() || !cellToLeft.HasRightWall())
                yield return cellToLeft;
        }

        // Check the top neighbor
        if (y + 1 < _mazeHeight)
        {
            var cellToTop = mazeGrid[x, y + 1];
            if (!currentCell.HasFrontWall() || !cellToTop.HasBackWall())
                yield return cellToTop;
        }

        // Check the bottom neighbor
        if (y - 1 >= 0)
        {
            var cellToBottom = mazeGrid[x, y - 1];
            if (!currentCell.HasBackWall() || !cellToBottom.HasFrontWall())
                yield return cellToBottom;
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
            return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRight();
            currentCell.ClearLeft();
            return;
        }
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeft();
            currentCell.ClearRight();
            return;
        }

        if (previousCell.transform.position.y < currentCell.transform.position.y)
        {
            previousCell.ClearFront();
            currentCell.ClearBack();
            return;
        }

        if (previousCell.transform.position.y > currentCell.transform.position.y)
        {
            previousCell.ClearBack();
            currentCell.ClearFront();
            return;
        }
    }

    public void ActivateShortestPath()
    {
        foreach (MazeCell cell in shortestPath)
        {
            cell.EnableShortedBlock();
        }
    }

    public void DeactivateShortestPath()
    {
        foreach (MazeCell cell in shortestPath)
        {
            cell.DisableShortedBlock();
        }
    }
}
