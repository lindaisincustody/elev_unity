using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Parameters")]
    [SerializeField] private int cellsPerEnemy = 5;
    [SerializeField] private PatrolEnemy enemyPrefab;
    [SerializeField] private ExpandingEnemy expEnemyPrefab;

    [SerializeField] private int expandingEnemyTicketCount;
    [SerializeField] private int followingEnemyTicketCount;
    [Header("Path Shower")]
    [SerializeField] private PathConsumable pathConsumable;

    List<PatrolEnemy> createdPatrolEnemies = new List<PatrolEnemy>();
    PatrolEnemy chosenEnemy;

    private int enemyCellsVariation = 2;

    private int _mazeWidth;
    private int _mazeHeight;
    private int cellSize = 4;

    public void SetMazeParameters(int mazeWidth, int mazeHeight, int mazeCellSize)
    {
        _mazeHeight = mazeHeight;
        _mazeWidth = mazeWidth;
        cellSize = mazeCellSize;
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                if ((j - enemyCellsVariation) % cellsPerEnemy == 0 && (i - enemyCellsVariation) % cellsPerEnemy == 0)
                {
                    CreateEnemy(i, j);
                    PathConsumable newConsumable = Instantiate(pathConsumable, new Vector2(i * cellSize, j * cellSize), Quaternion.identity);
                    newConsumable.transform.parent = gameObject.transform;
                }
            }
        }
    }

    private void CreateEnemy(int i, int j)
    {
        int xPosVariation = Random.Range(-enemyCellsVariation, enemyCellsVariation);
        int yPosVariation = Random.Range(-enemyCellsVariation, enemyCellsVariation);

        Vector2 enemyPos = new Vector2((i * cellSize + xPosVariation * cellSize), (j * cellSize + yPosVariation * cellSize));

        BaseEnemy enemy = ChooseEnemyToSpawn();
        BaseEnemy exp = Instantiate(enemy, enemyPos, Quaternion.identity);
        if (exp.GetComponent<ExpandingEnemy>() != null)
            exp.GetComponent<ExpandingEnemy>().SetCellCoordinates(i + xPosVariation, j + yPosVariation, cellSize);
        else
            createdPatrolEnemies.Add((PatrolEnemy)exp);
        exp.transform.parent = gameObject.transform;
    }

    private BaseEnemy ChooseEnemyToSpawn()
    {
        int totalTickets = expandingEnemyTicketCount + followingEnemyTicketCount;
        if (totalTickets <= 0)
        {
            return null; // No tickets available
        }

        int chosenTicket = Random.Range(0, totalTickets);
        if (chosenTicket < expandingEnemyTicketCount)
        {
            return expEnemyPrefab; // Chose an expanding enemy
        }
        else
        {
            return enemyPrefab; // Chose a following enemy
        }
    }

    public Vector3? ChoosePatrolEnemyToRageMode()
    {
        int chosenEnemyIndex = Random.Range(0, createdPatrolEnemies.Count - 1);
        chosenEnemy = createdPatrolEnemies[chosenEnemyIndex];
        if (chosenEnemy != null)
        {
            chosenEnemy.HideEnemy();
            return chosenEnemy.transform.position;
        }
        return null;
    }

    public void ExitRageMode(Vector3 mainEnemyPos)
    {
        chosenEnemy.AppearEnemy(mainEnemyPos);
    }
}
