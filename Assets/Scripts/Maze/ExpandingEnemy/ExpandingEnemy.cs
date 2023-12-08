using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingEnemy : MonoBehaviour
{
    public GameObject tenticle;
    public MazeGenerator mazeGenerator;
    private int tenticleLength = 4;
    private int xCell;
    private int yCell;
    private int rightCellXPos;
    private int rightCellYPos;
    private int leftCellXPos;
    private int leftCellYPos;
    private int upCellXPos;
    private int upCellYPos;
    private int downCellXPos;
    private int downCellYPos;
    private int tencacleOffset;

    private void Start()
    {
        mazeGenerator = FindFirstObjectByType<MazeGenerator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Expand();
    }

    public void SetCellCoordinates(int x, int y, int cellSize)
    {
        xCell = (int)gameObject.transform.position.x; 
        yCell = (int)gameObject.transform.position.y;
        rightCellXPos = x; leftCellXPos = x; upCellXPos = x; downCellXPos = x;
        rightCellYPos = y; leftCellYPos = y; upCellYPos = y; downCellYPos = y;
        tencacleOffset = cellSize;
    }

    private void Expand()
    {
        int rightIndex = 1;
        Vector2 TenticlePos;
        Direction lastDirection = Direction.Left;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = RightTentacle(rightCellXPos, rightCellYPos, ref rightIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Right)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = Instantiate(tenticle, TenticlePos, Quaternion.identity, gameObject.transform);
                xCell = (int)ten.transform.position.x;
                yCell = (int)ten.transform.position.y;
            }
            else
                break;
        }
        xCell = (int)gameObject.transform.position.x;
        yCell = (int)gameObject.transform.position.y;
        int leftIndex = 1;
        lastDirection = Direction.Right;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = LeftTentacle(leftCellXPos, leftCellYPos, ref leftIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Left)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = Instantiate(tenticle, TenticlePos, Quaternion.identity, gameObject.transform);
                xCell = (int)ten.transform.position.x;
                yCell = (int)ten.transform.position.y;
            }
            else
                break;
        }

        xCell = (int)gameObject.transform.position.x;
        yCell = (int)gameObject.transform.position.y;
        int upIndex = 1;
        lastDirection = Direction.Down;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = UpTentacle(upCellXPos, upCellYPos, ref upIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Up)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = Instantiate(tenticle, TenticlePos, Quaternion.identity, gameObject.transform);
                xCell = (int)ten.transform.position.x;
                yCell = (int)ten.transform.position.y;
            }
            else
                break;
        }

        xCell = (int)gameObject.transform.position.x;
        yCell = (int)gameObject.transform.position.y;
        int downIndex = 1;
        lastDirection = Direction.Up;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = DownTentacle(downCellXPos, downCellYPos, ref downIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Down)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = Instantiate(tenticle, TenticlePos, Quaternion.identity, gameObject.transform);
                xCell = (int)ten.transform.position.x;
                yCell = (int)ten.transform.position.y;
            }
            else
                break;
        }
    }

    private Vector2 DownTentacle(int x, int y, ref int i, ref Direction lastDir)
    {
        MazeCell downCell = mazeGenerator.GetMazeCell(x, y);
        if (downCell == null)
            return new Vector2(-1, -1);
        if (!downCell.HasBackWall())
        {
            lastDir = Direction.Down;
            downCellYPos--;
            return new Vector2(xCell, yCell - i * tencacleOffset);
        }
        if (!downCell.HasRightWall() && lastDir != Direction.Left)
        {
            lastDir = Direction.Right;
            downCellXPos++;
            return new Vector2(xCell + i * tencacleOffset, yCell);
        }
        if (!downCell.HasLeftWall() && lastDir != Direction.Right)
        {
            lastDir = Direction.Left;
            downCellXPos--;
            return new Vector2(xCell - i * tencacleOffset, yCell);
        }
        lastDir = Direction.Right;
        return new Vector2(-1, -1);
    }

    private Vector2 UpTentacle(int x, int y, ref int i, ref Direction lastDir)
    {
        MazeCell upCell = mazeGenerator.GetMazeCell(x, y);
        if (upCell == null)
            return new Vector2(-1, -1);
        if (!upCell.HasFrontWall())
        {
            lastDir = Direction.Up;
            upCellYPos++;
            return new Vector2(xCell, yCell + i * tencacleOffset);
        }
        if (!upCell.HasLeftWall() && lastDir != Direction.Right)
        {
            lastDir = Direction.Left;
            upCellXPos--;
            return new Vector2(xCell - i * tencacleOffset, yCell);
        }
        if (!upCell.HasRightWall() && lastDir != Direction.Left)
        {
            lastDir = Direction.Right;
            upCellXPos++;
            return new Vector2(xCell + i * tencacleOffset, yCell);
        }
        lastDir = Direction.Right;
        return new Vector2(-1, -1);
    }

    private Vector2 LeftTentacle(int x, int y, ref int i, ref Direction lastDir)
    {
        MazeCell leftCell = mazeGenerator.GetMazeCell(x, y);
        if (leftCell == null)
            return new Vector2(-1, -1);
        if (!leftCell.HasLeftWall())
        {
            lastDir = Direction.Left;
            leftCellXPos--;
            return new Vector2(xCell - i * tencacleOffset, yCell);
        }
        if (!leftCell.HasFrontWall() && lastDir != Direction.Down)
        {
            lastDir = Direction.Up;
            leftCellYPos++;
            return new Vector2(xCell, yCell + i * tencacleOffset);
        }
        if (!leftCell.HasBackWall() && lastDir != Direction.Up)
        {
            lastDir = Direction.Down;
            leftCellYPos--;
            return new Vector2(xCell, yCell - i * tencacleOffset);
        }
        lastDir = Direction.Right;
        return new Vector2(-1, -1);
    }

    private Vector2 RightTentacle(int x, int y, ref int i, ref Direction lastDir)
    {
        MazeCell rightCell = mazeGenerator.GetMazeCell(x, y);
        if (rightCell == null)
            return new Vector2(-1, -1);
        if (!rightCell.HasRightWall())
        {
            lastDir = Direction.Right;
            rightCellXPos++;
            return new Vector2(xCell + i * tencacleOffset, yCell);
        }
        if (!rightCell.HasBackWall() && lastDir != Direction.Up)
        {
            lastDir = Direction.Down;
            rightCellYPos--;
            return new Vector2(xCell, yCell - i * tencacleOffset);
        }
        if (!rightCell.HasFrontWall() && lastDir != Direction.Down)
        {
            lastDir = Direction.Up;
            rightCellYPos++;
            return new Vector2(xCell, yCell + i * tencacleOffset);
        }
        lastDir = Direction.Left;
        return new Vector2(-1, -1);
    }
}

public enum Direction
{
    Right,
    Down,
    Left,
    Up
}
