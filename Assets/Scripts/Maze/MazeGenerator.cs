using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Parameters")]
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeHeight;
    [SerializeField] private float mazeSpeed = 0.02f;

    [Header("Loops Parameters")]
    [SerializeField] private bool allowLoops = false;
    [SerializeField] private int numberOfLoops = 5; // Set the number of loops you want

    private MazeCell[,] mazeGrid;
    private int cellSize = 4;

    private IEnumerator Start()
    {
        mazeGrid = new MazeCell[_mazeWidth, _mazeHeight];
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                mazeGrid[i, j] = Instantiate(_mazeCellPrefab, new Vector2(i * cellSize, j * cellSize), Quaternion.identity);
            }
        }

        // Set the exit cell
        MazeCell exitCell = mazeGrid[_mazeWidth - 1, _mazeHeight - 2];

        yield return GenerateMaze(null, mazeGrid[0, 0], exitCell); // Pass the exit cell

        CalculateShortestPath(mazeGrid[0, 0], exitCell);
        // Randomly delete walls to create loops only once
        if (allowLoops)
        {
            CreateLoops();
        }
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

        queue.Enqueue(startCell);
        parentMap[startCell] = null;

        while (queue.Count > 0)
        {
            MazeCell currentCell = queue.Dequeue();

            if (currentCell == exitCell)
                break;

            foreach (var neighbor in GetNeighborsWithNoWalls(currentCell))
            {
                if (!parentMap.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    parentMap[neighbor] = currentCell;
                }
            }
        }

        // Reconstruct the shortest path from exitCell to startCell using parentMap
        List<MazeCell> shortestPath = new List<MazeCell>();
        MazeCell cell = exitCell;

        while (cell != null)
        {
            shortestPath.Add(cell);
            cell.SetAsShortestPath();
            if (parentMap.ContainsKey(cell))
            {
                cell = parentMap[cell];
            }
            else
            {
                break; // Exit the loop if there's no parent (e.g., startCell)
            }
        }

        shortestPath.Reverse(); // Reverse the path to start from startCell
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
}
