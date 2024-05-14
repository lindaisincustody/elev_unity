using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject xPrefab;
    public GameObject oPrefab;
    public Sprite correctDestinationSprite;

    public int ActualGameLevel = 50;

    private int currentLevel = 50; // Start with level 1
    private int difficultyLevel = 50; // overall difficulty

    public TMP_Text levelCurrentText;
    public TMP_Text levelsWonText;
    public TMP_Text levelsLostText;
    public int lostLevels = 0;
    public int wonLevels = 0;

    private List<Vector2Int> oPositions;
    private Vector2Int correctDestination;

    private int gridSizeX = 20;
    private int gridSizeY = 20;

    private DataManager dataManager;

    void Start()
    {
        dataManager = DataManager.Instance;

        ActualGameLevel = dataManager.GetLevel(Attribute.Intelligence);
        currentLevel = ActualGameLevel;
        difficultyLevel = ActualGameLevel;
        GenerateLevel();
    }

    void GenerateLevel()
    {
        DestroyOldLevel(); // Destroy the old level before generating a new one

        bool solvableLevelGenerated = false;
        int maxRetries = 20; // Adjust the maximum number of retries as needed

        for (int retryCount = 0; retryCount < maxRetries; retryCount++)
        {
            GenerateOPositions();

            solvableLevelGenerated = IsSolvable();

            if (solvableLevelGenerated)
            {
                // Successfully generated a solvable level, exit the loop
                break;
            }
            else
            {
                // Level generation failed, reset and try again
                oPositions.Clear();
                oPositions.Add(new Vector2Int(0, 0));
            }
        }

        if (!solvableLevelGenerated)
        {
            // Handle the situation where a solvable level could not be generated within the maximum retries
            
            Debug.LogError("Failed to generate a solvable level within the maximum retries.");
            Application.Quit();
        }
    }

    void DestroyOldLevel()
    {
        // Destroy all game objects with either "O" or "X" tag
        GameObject[] oldObjects = GameObject.FindGameObjectsWithTag("O").Concat(GameObject.FindGameObjectsWithTag("X")).Concat(GameObject.FindGameObjectsWithTag("CorrectDestination")).ToArray();

        foreach (GameObject oldObject in oldObjects)
        {
            Destroy(oldObject);
        }
    }

    void GenerateOPositions()
    {
        oPositions = new List<Vector2Int>();
        Vector2Int playerStartPosition = new Vector2Int(0, 0);
        oPositions.Add(playerStartPosition);

        bool destinationFound = false;
        int maxRetries = 100; // Adjust the maximum number of retries as needed

        // Attempt to find solvable level with a valid destination
        for (int retryCount = 0; retryCount < maxRetries && !destinationFound; retryCount++)
        {
            // Generate positions for O's using recursive backtracking
            for (int i = 1; i <= difficultyLevel; i++)
            {
                Vector2Int previousPosition = oPositions[i - 1];
                Vector2Int nextPosition = GenerateNextPosition(previousPosition);
                oPositions.Add(nextPosition);
            }

            // Randomly select one O position as the correct destination
            correctDestination = oPositions[oPositions.Count - 1];

            // Check if the correct destination is reachable and the level is solvable
            if (CanReachDestination(playerStartPosition, correctDestination) && IsSolvable())
            {
                destinationFound = true;

                // Draw the solvable path for visualization
                DrawSolvablePath(oPositions);
            }
            else
            {
                // If the level is not solvable, reset positions and try again
                oPositions.Clear();
                oPositions.Add(playerStartPosition);
            }
        }

        if (!destinationFound)
        {
            // Handle the situation where a solvable level could not be generated within the maximum retries
            Debug.LogError("Failed to generate a solvable level within the maximum retries.");
        }

        // Instantiate O prefabs at generated positions
        for (int i = 0; i < oPositions.Count; i++)
        {
            Vector2Int position = oPositions[i];
            GameObject oObject = Instantiate(oPrefab, new Vector3(position.x - 4, position.y - 3, 0), Quaternion.identity);

            if (position == correctDestination)
            {
                oObject.GetComponent<SpriteRenderer>().sprite = correctDestinationSprite;
                oObject.tag = "CorrectDestination";
                break; // Exit the loop after processing the correct destination
            }
        }
    }
    Vector2Int GenerateNextPosition(Vector2Int currentPosition)
    {
        List<Vector2Int> validNeighbors = GetValidNeighbors(currentPosition);
        ShuffleList(validNeighbors);

        foreach (Vector2Int neighbor in validNeighbors)
        {
            if (!oPositions.Contains(neighbor))
            {
                return neighbor;
            }
        }

        // If no valid neighbor is found, return the current position
        return currentPosition;
    }

    List<Vector2Int> GetValidNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(position.x + 2, position.y),
            new Vector2Int(position.x - 2, position.y),
            new Vector2Int(position.x, position.y + 2),
            new Vector2Int(position.x, position.y - 2)
        };

        // Filter out positions that are out of bounds or have already been used
        return neighbors.Where(pos => IsValidPosition(pos)).ToList();
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void Update()
    {
        //IntelligencePlayerController playerController = FindObjectOfType<IntelligencePlayerController>();
        //Vector2Int playerPosition = playerController.GetCurrentPosition();
        //Vector2Int adjustedDestination = new Vector2Int(correctDestination.x - 4, correctDestination.y - 3);

        //// Check if the player is within a 1-unit range of the adjusted destination
        //if (Mathf.Abs(playerPosition.x - adjustedDestination.x) <= 1 &&
        //    Mathf.Abs(playerPosition.y - adjustedDestination.y) <= 1)
        //{
        //    Invoke("CheckWinCondition", 0.4f);
        //}

        //Debug.Log(correctDestination);
        UpdateWonLevelsText();
        UpdateLostLevelsText();
        UpdateCurrentLevelsText();

        CheckOverallWin();
    }

    public void CheckOverallWin()
    {
        if (currentLevel > 10)
        {
            if (wonLevels - lostLevels >= currentLevel / 3)
            {
                ActualGameLevel++;
                dataManager.AddLevel(Attribute.Intelligence);
                Debug.Log("You won!");
                SceneManager.LoadScene(DataManager.Instance.GetLastScene());
            }
            else
            {
                Debug.Log("You Lost!");
                SceneManager.LoadScene(DataManager.Instance.GetLastScene());
            }
        }
    }
    public void CheckWinCondition()
    {
        IntelligencePlayerController playerController = FindObjectOfType<IntelligencePlayerController>();
        Vector2Int playerPosition = playerController.GetCurrentPosition();
        Vector2Int adjustedDestination = new Vector2Int(correctDestination.x - 4, correctDestination.y - 3);

        // Check if the player is on the correct destination
        //if (playerPosition == adjustedDestination)
        //{
        GameObject[] objectsWithTagO = GameObject.FindGameObjectsWithTag("O");

        // If no objects with tag "O" are found, return true (all converted)
        if (objectsWithTagO.Length == 0)
        {
            Debug.Log("You won! Reached the correct destination: " + correctDestination);
            IncreaseLevelWon();
            IncreaseLevel();

            // Teleport the player to the specified position
            playerController.TeleportPlayer(new Vector2Int(-4, -3));

            GenerateLevel(); // Generate the next level
        }
        else
        {
            Debug.Log("Not all O's turned into X. Try again!");
            IncreaseLevelLost();
            IncreaseLevel();
            playerController.TeleportPlayer(new Vector2Int(-4, -3));
            GenerateLevel();
        }
    }

    void UpdateWonLevelsText()
    {
        // Update the TMP text to display the current value of wonLevels
        levelsWonText.text = wonLevels.ToString();
    }
    void UpdateLostLevelsText()
    {
        // Update the TMP text to display the current value of wonLevels
        levelsLostText.text = lostLevels.ToString();
    }
    void UpdateCurrentLevelsText()
    {
        // Update the TMP text to display the current value of wonLevels
        levelCurrentText.text = "Current Level: " + currentLevel.ToString();
    }

    void IncreaseLevel()
    {
        currentLevel++;
        if(currentLevel % 3 == 0)
            difficultyLevel++;
        //gridSizeX= gridSizeX + 2;
        //gridSizeY = gridSizeY + 2;
    }
    void IncreaseLevelWon()
    {
        wonLevels++;
    }
    void IncreaseLevelLost()
    {
        lostLevels++; 
    }

    bool CanReachDestination(Vector2Int start, Vector2Int destination)
    {
        List<Node> path = AStarPathfinding(start, destination);

        // If a valid path exists, return true
        return path != null && path.Count > 0;
    }

    List<Node> AStarPathfinding(Vector2Int start, Vector2Int destination)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = new Node(start, null, destination);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = GetLowestFScoreNode(openSet);

            if (currentNode.Position == destination)
            {
                // Found a path
                return ReconstructPath(currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Vector2Int neighborPosition in GetNeighbors(currentNode.Position))
            {
                Node neighbor = new Node(neighborPosition, currentNode, destination);

                if (closedSet.Contains(neighbor) || !IsValidPosition(neighborPosition))
                {
                    continue;
                }

                int tentativeGScore = currentNode.GScore + 1;

                if (!openSet.Contains(neighbor) || tentativeGScore < neighbor.GScore)
                {
                    neighbor.GScore = tentativeGScore;
                    neighbor.HScore = HeuristicCostEstimate(neighbor.Position, destination);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path found
        return null;
    }

    Node GetLowestFScoreNode(List<Node> nodes)
    {
        Node lowestNode = nodes[0];

        foreach (Node node in nodes)
        {
            if (node.FScore < lowestNode.FScore)
            {
                lowestNode = node;
            }
        }

        return lowestNode;
    }

    List<Node> ReconstructPath(Node node)
    {
        List<Node> path = new List<Node>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    int HeuristicCostEstimate(Vector2Int current, Vector2Int goal)
    {
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        neighbors.Add(new Vector2Int(position.x + 1, position.y));
        neighbors.Add(new Vector2Int(position.x - 1, position.y));
        neighbors.Add(new Vector2Int(position.x, position.y + 1));
        neighbors.Add(new Vector2Int(position.x, position.y - 1));
        return neighbors;
    }

    bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeY;
    }

    class Node
    {
        public Vector2Int Position { get; }
        public Node Parent { get; }
        public int GScore { get; set; }
        public int HScore { get; set; }
        public int FScore => GScore + HScore;

        public Node(Vector2Int position, Node parent, Vector2Int goal)
        {
            Position = position;
            Parent = parent;
            GScore = parent != null ? parent.GScore + 1 : 0;
            HScore = HeuristicCostEstimate(position, goal);
        }

        int HeuristicCostEstimate(Vector2Int current, Vector2Int goal)
        {
            return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
        }
    }

    bool IsSolvable()
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int playerStartPosition = new Vector2Int(0, 0);

        // Perform DFS from the player's starting position
        DFS(playerStartPosition, visited);

        // Check if all O positions (excluding the correct destination) are visited exactly once
        foreach (Vector2Int position in oPositions)
        {
            if (position != correctDestination)
            {
                if (!visited.Contains(position) || visited.Count(pos => pos == position) > 1)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void DFS(Vector2Int current, HashSet<Vector2Int> visited)
    {
        visited.Add(current);

        foreach (Vector2Int neighbor in GetNeighbors(current))
        {
            if (IsValidPosition(neighbor) && !visited.Contains(neighbor))
            {
                DFS(neighbor, visited);
            }
        }
    }

    void DrawSolvablePath(List<Vector2Int> positions)
    {
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector2Int start = new Vector2Int(positions[i].x - 4, positions[i].y - 3);
            Vector2Int end = new Vector2Int(positions[i + 1].x - 4, positions[i + 1].y - 3);

            Debug.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0), Color.green, 2f);
        }
    }
}

