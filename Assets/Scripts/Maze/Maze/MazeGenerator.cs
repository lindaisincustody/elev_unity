using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NavMeshPlus.Components;
using UnityEngine.Events;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] public NavMeshSurface navmesh;
    [SerializeField] public NavMeshCreator creator;
    [SerializeField] public CameraTransition cameraTransition;
    [SerializeField] public GameObject endPrefab;

    [Header("Maze Parameters")]
    [SerializeField] public MazeCell _mazeCellPrefab;

    [Header("Loops Parameters")]
    [SerializeField] private bool allowLoops = false;
    [SerializeField] private int numberOfSquaresForALoop = 5;

    public int _mazeWidth;
    public int _mazeHeight;
    public MazeCell[,] mazeGrid;
    private int cellSize = 4;
    public List<MazeCell> shortestPath;
    [System.NonSerialized] public UnityEvent OnMazeCompletion = new UnityEvent();

    private EnemySpawner enemySpawner;
    private PathShowersSpawner pathShowerSpawner;
    private ImageHolder imageHolder;
    public float timer = 0.0f;

    private void Awake()
    {
        imageHolder = GetComponent<ImageHolder>();
        enemySpawner = GetComponent<EnemySpawner>();
        pathShowerSpawner = GetComponent<PathShowersSpawner>();
    }

    public void StartGeneratingMaze(int mazeWidth, int mazeHeight)
    {
        _mazeWidth = mazeWidth;
        _mazeHeight = mazeHeight;
        StartCoroutine(BuildMaze());
    }

    private IEnumerator BuildMaze()
    {
        //cameraTransition.SetCamera(_mazeWidth, _mazeHeight, cellSize);
        //enemySpawner.SetMazeParameters(_mazeWidth, _mazeHeight, cellSize);
        //pathShowerSpawner.SetMazeParameters(_mazeWidth, _mazeHeight, cellSize);
        creator.ResizeNavMesh(_mazeWidth * cellSize, _mazeHeight * cellSize);
        mazeGrid = new MazeCell[_mazeWidth, _mazeHeight];
        numberOfSquaresForALoop = (_mazeHeight * _mazeWidth)/numberOfSquaresForALoop;
        //_mazeCellPrefab = GameObject.FindAnyObjectByType<MazeCell>();
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                MazeCell newCell = GameObject.Instantiate(new GameObject()).AddComponent<MazeCell>();
                //newCell.transform.position = new Vector2(i * cellSize, j * cellSize);
                //MazeCell newCell = Instantiate(_mazeCellPrefab, new Vector2(i * cellSize, j * cellSize), Quaternion.identity);
                mazeGrid[i, j] = newCell;
                newCell.transform.parent = gameObject.transform;
            }
        }

        // Set the exit cell
        MazeCell exitCell = mazeGrid[_mazeWidth - 1, _mazeHeight - 2];
        //Instantiate(endPrefab, new Vector2(exitCell.transform.position.x + cellSize, exitCell.transform.position.y), Quaternion.identity);
        //newCell = GameObject.Instantiate(new GameObject()).AddComponent<MazeCell>();
        GenerateMaze(null, mazeGrid[0, 0], exitCell); // Pass the exit cell
                                                      // Randomly delete walls to create loops only once
        if (allowLoops)
        {
            CreateLoops();
        }
        //CalculateShortestPath(mazeGrid[0, 0], exitCell);
        //enemySpawner.SpawnEnemies();
        //pathShowerSpawner.SpawnPathShowers();
        yield return new WaitForEndOfFrame();
        //cameraTransition.StartZoomIn();

        OnMazeCompletion?.Invoke();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            // If the timer reaches zero or below, deactivate the path
            if (timer <= 0)
            {
                timer = 0; // Reset timer to 0 for safety
                DeactivateShortestPath();
            }
        }
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell, MazeCell exitCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell, exitCell);
            }
        } while (nextCell != null);

        // Check if the current cell is the exit cell, and mark it as the exit
        if (currentCell == exitCell)
        {
            currentCell.MarkAsExit(_mazeWidth, _mazeHeight, mazeGrid);
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
        while (loopsCreated < numberOfSquaresForALoop)
        {
            int randomX = Random.Range(1, _mazeWidth - 1);
            int randomY = Random.Range(1, _mazeHeight - 1);

            var randomCell = mazeGrid[randomX, randomY];
            if (randomCell != null)
            {
                randomCell.DeleteRandomWall(randomX, randomY, mazeGrid);
                loopsCreated++;
            }
        }
    }

    public void CalculateShortestPath(MazeCell startCell, MazeCell exitCell)
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
            currentCell.SetDirection(imageHolder.CIrcle);
        }
        currentCell.DisableShortedBlock();
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

    public void Activate(float addedTime)
    {
        timer += addedTime;
        ActivateShortestPath();
    }

    private void ActivateShortestPath()
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

    public MazeCell GetMazeCell(int x, int y)
    {
        if (x >= _mazeWidth || y >= _mazeHeight)
            return null;
        return mazeGrid[x, y];
    }

    public Vector2 GetRandomCellPosition()
    {
        int x = Random.Range(0, _mazeWidth - 1);
        int y = Random.Range(0, _mazeHeight - 1);

        return mazeGrid[x, y].transform.position;
    }

    public void HideAllEnemies()
    {
        enemySpawner.HideEnemies();
    }
}
