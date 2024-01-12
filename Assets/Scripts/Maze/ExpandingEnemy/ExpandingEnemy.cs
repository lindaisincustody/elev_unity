using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingEnemy : BaseEnemy
{
    public Material BeamEffectMat;
    public GameObject effectObj;
    public float smallSize = 1f;
    public float bigSize = 3f;
    public GameObject tenticle;
    public float expandingCooldown = 5f;
    public float expadingDuration = 8f;
    public float preparationDuration = 2f;
    List<Tuple<GameObject, int>> tencticles = new List<Tuple<GameObject, int>>();
    public List<GameObject> availableTentacles = new List<GameObject>();

    private float tecticleExpandDelay = 1f;
    private int tenticleLength = 4;

    private int persistentX;
    private int persistentY;

    private int xCell;
    private int yCell;
    private int xRCell; private int xLCell; private int xUCell; private int XDCell;
    private int yRCell; private int yLCell; private int yUCell; private int YDCell;
    private int rightCellXPos;
    private int rightCellYPos;
    private int leftCellXPos;
    private int leftCellYPos;
    private int upCellXPos;
    private int upCellYPos;
    private int downCellXPos;
    private int downCellYPos;
    private int tencacleOffset;

    private Transform downTenticle;
    private Transform leftTentcile;
    private Transform upTenticle;
    private Transform rightTenticle;

    private ParticleSystem ps;

    public Rigidbody2D rb;

    private void Start()
    {
        ps = effectObj.GetComponent<ParticleSystem>();
    }

    public override void ActivateEnemy()
    {
        StartCoroutine(AbilitiesLoop());
    }

    public void PlayerHit(GameObject player)
    {
        player.GetComponent<MazePlayerMovement>().LeashToObject(rb, transform);
        float factor = Mathf.Pow(2, 8);
        Color color = new Color(0.01f * factor, 0.8f * factor, 0.7f * factor);
        BeamEffectMat.SetColor("_Color", color);
        StartCoroutine(ResetColor());
    }

    public IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(7f);
        float factor = Mathf.Pow(2, 8);
        Color color = new Color(0.8f * factor, 0.01f * factor, 0.01f * factor);
        BeamEffectMat.SetColor("_Color", color);
    }

    public void ParticleExpansionCycle()
    {
        StartCoroutine(IncreaseParticleSize());
    }

    private IEnumerator IncreaseParticleSize()
    {
        float elapsedTime = 0f;
        var mainModule = ps.main;
        // Loop over 1 second
        while (elapsedTime < preparationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newSize = Mathf.Lerp(smallSize, bigSize, elapsedTime / preparationDuration);
            mainModule = ps.main;
            mainModule.startSize = newSize;

            yield return null;
        }
        mainModule = ps.main;
        mainModule.startSize = bigSize;

        yield return new WaitForSeconds(expadingDuration);
        StartCoroutine(DecreaseParticleSize());
    }

    private IEnumerator DecreaseParticleSize()
    {
        float elapsedTime = 0f;
        var mainModule = ps.main;
        // Loop over 1 second
        while (elapsedTime < preparationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newSize = Mathf.Lerp(bigSize, smallSize, elapsedTime / preparationDuration);
            mainModule = ps.main;
            mainModule.startSize = newSize;

            yield return null;
        }
        mainModule = ps.main;
        mainModule.startSize = smallSize;
    }

    private IEnumerator AbilitiesLoop()
    {
        yield return new WaitForSeconds(expandingCooldown);
        ParticleExpansionCycle();
        yield return new WaitForSeconds(preparationDuration);
        StartCoroutine(ExpandToRight());
        StartCoroutine(ExpandToLeft());
        StartCoroutine(ExpandToUp());
        StartCoroutine(ExpandToDown());
        yield return new WaitForSeconds(expadingDuration);
        StartCoroutine(Condense());
        StartCoroutine(AbilitiesLoop());
    }

    private IEnumerator Condense()
    {
        SetCellCoordinates(persistentX, persistentY, tencacleOffset);
        for (int i = 4; i > 0; i--)
        {
            for (int j = 0; j < 4; j++)
            {
                if (tencticles.Count > 0)
                {
                    if (tencticles[tencticles.Count - 1].Item2 == i)
                    {
                        GameObject disabledTentacle = tencticles[tencticles.Count - 1].Item1.gameObject;
                        disabledTentacle.SetActive(false);
                        availableTentacles.Add(disabledTentacle);
                        tencticles.RemoveAt(tencticles.Count - 1);
                    }
                }
            }

            yield return new WaitForSeconds(tecticleExpandDelay);
        }
        rightTenticle = null; leftTentcile = null; upTenticle = null; downTenticle = null;
    }

    public void SetCellCoordinates(int x, int y, int cellSize)
    {
        persistentX = x; persistentY = y;
        xCell = (int)gameObject.transform.position.x; 
        yCell = (int)gameObject.transform.position.y;
        XDCell = xCell; xUCell = xCell; xRCell = xCell; xLCell = xCell;
        YDCell = yCell; yUCell = yCell; yRCell = yCell; yLCell = yCell;
        rightCellXPos = x; leftCellXPos = x; upCellXPos = x; downCellXPos = x;
        rightCellYPos = y; leftCellYPos = y; upCellYPos = y; downCellYPos = y;
        tencacleOffset = cellSize;
    }

    private IEnumerator ExpandToRight()
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
                GameObject ten = availableTentacles[availableTentacles.Count - 1];
                availableTentacles.RemoveAt(availableTentacles.Count - 1);
                ten.SetActive(true);
                ten.transform.position = TenticlePos;

                if (rightTenticle == null)
                    ten.GetComponent<Connection>().SetTarget(gameObject.transform);
                else
                    ten.GetComponent<Connection>().SetTarget(rightTenticle);
                tencticles.Add(new Tuple<GameObject, int>(ten, i));
                rightTenticle = ten.transform;
                xRCell = (int)ten.transform.position.x;
                yRCell = (int)ten.transform.position.y;
            }
            else
                break;
            yield return new WaitForSeconds(tecticleExpandDelay);
        }
    }

    private IEnumerator ExpandToLeft()
    {
        Vector2 TenticlePos;
        Direction lastDirection = Direction.Right;
        int leftIndex = 1;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = LeftTentacle(leftCellXPos, leftCellYPos, ref leftIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Left)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = availableTentacles[availableTentacles.Count - 1];
                availableTentacles.RemoveAt(availableTentacles.Count - 1);
                ten.SetActive(true);
                ten.transform.position = TenticlePos;

                if (leftTentcile == null)
                    ten.GetComponent<Connection>().SetTarget(gameObject.transform);
                else
                    ten.GetComponent<Connection>().SetTarget(leftTentcile);
                tencticles.Add(new Tuple<GameObject, int>(ten, i));
                leftTentcile = ten.transform;
                xLCell = (int)ten.transform.position.x;
                yLCell = (int)ten.transform.position.y;
            }
            else
                break;
            yield return new WaitForSeconds(tecticleExpandDelay);
        }
    }

    private IEnumerator ExpandToUp()
    {
        Vector2 TenticlePos;
        Direction lastDirection = Direction.Down;
        int upIndex = 1;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = UpTentacle(upCellXPos, upCellYPos, ref upIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Up)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = availableTentacles[availableTentacles.Count - 1];
                availableTentacles.RemoveAt(availableTentacles.Count - 1);
                ten.SetActive(true);
                ten.transform.position = TenticlePos;

                if (upTenticle == null)
                    ten.GetComponent<Connection>().SetTarget(gameObject.transform);
                else
                    ten.GetComponent<Connection>().SetTarget(upTenticle);
                tencticles.Add(new Tuple<GameObject, int>(ten, i));
                upTenticle = ten.transform;
                xUCell = (int)ten.transform.position.x;
                yUCell = (int)ten.transform.position.y;
            }
            else
                break;
            yield return new WaitForSeconds(tecticleExpandDelay);
        }
    }

    private IEnumerator ExpandToDown()
    {
        Vector2 TenticlePos;
        Direction lastDirection = Direction.Up;
        int downIndex = 1;
        for (int i = 1; i < tenticleLength + 1; i++)
        {
            TenticlePos = DownTentacle(downCellXPos, downCellYPos, ref downIndex, ref lastDirection);
            if (i == 1 && lastDirection != Direction.Down)
                break;
            if (TenticlePos != new Vector2(-1, -1))
            {
                GameObject ten = availableTentacles[availableTentacles.Count - 1];
                availableTentacles.RemoveAt(availableTentacles.Count - 1);
                ten.SetActive(true);
                ten.transform.position = TenticlePos;

                if (downTenticle == null)
                    ten.GetComponent<Connection>().SetTarget(gameObject.transform);
                else
                    ten.GetComponent<Connection>().SetTarget(downTenticle);
                tencticles.Add(new Tuple<GameObject, int>(ten, i));
                downTenticle = ten.transform;
                XDCell = (int)ten.transform.position.x;
                YDCell = (int)ten.transform.position.y;
            }
            else
                break;
            yield return new WaitForSeconds(tecticleExpandDelay);
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
            return new Vector2(XDCell, YDCell - i * tencacleOffset);
        }
        if (!downCell.HasRightWall() && lastDir != Direction.Left)
        {
            lastDir = Direction.Right;
            downCellXPos++;
            return new Vector2(XDCell + i * tencacleOffset, YDCell);
        }
        if (!downCell.HasLeftWall() && lastDir != Direction.Right)
        {
            lastDir = Direction.Left;
            downCellXPos--;
            return new Vector2(XDCell - i * tencacleOffset, YDCell);
        }
        if (!downCell.HasFrontWall() && lastDir != Direction.Down)
        {
            lastDir = Direction.Up;
            downCellYPos++;
            return new Vector2(XDCell, YDCell + i * tencacleOffset);
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
            return new Vector2(xUCell, yUCell + i * tencacleOffset);
        }
        if (!upCell.HasLeftWall() && lastDir != Direction.Right)
        {
            lastDir = Direction.Left;
            upCellXPos--;
            return new Vector2(xUCell - i * tencacleOffset, yUCell);
        }
        if (!upCell.HasRightWall() && lastDir != Direction.Left)
        {
            lastDir = Direction.Right;
            upCellXPos++;
            return new Vector2(xUCell + i * tencacleOffset, yUCell);
        }
        if (!upCell.HasBackWall() && lastDir != Direction.Up)
        {
            lastDir = Direction.Down;
            upCellYPos--;
            return new Vector2(xUCell, yUCell - i * tencacleOffset);
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
            return new Vector2(xLCell - i * tencacleOffset, yLCell);
        }
        if (!leftCell.HasFrontWall() && lastDir != Direction.Down)
        {
            lastDir = Direction.Up;
            leftCellYPos++;
            return new Vector2(xLCell, yLCell + i * tencacleOffset);
        }
        if (!leftCell.HasBackWall() && lastDir != Direction.Up)
        {
            lastDir = Direction.Down;
            leftCellYPos--;
            return new Vector2(xLCell, yLCell - i * tencacleOffset);
        }
        if (!leftCell.HasRightWall() && lastDir != Direction.Left)
        {
            lastDir = Direction.Right;
            leftCellXPos++;
            return new Vector2(xLCell + i * tencacleOffset, yLCell);
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
            return new Vector2(xRCell + i * tencacleOffset, yRCell);
        }
        if (!rightCell.HasBackWall() && lastDir != Direction.Up)
        {
            lastDir = Direction.Down;
            rightCellYPos--;
            return new Vector2(xRCell, yRCell - i * tencacleOffset);
        }
        if (!rightCell.HasFrontWall() && lastDir != Direction.Down)
        {
            lastDir = Direction.Up;
            rightCellYPos++;
            return new Vector2(xRCell, yRCell + i * tencacleOffset);
        }
        if (!rightCell.HasLeftWall() && lastDir != Direction.Right)
        {
            lastDir = Direction.Left;
            rightCellXPos--;
            return new Vector2(xRCell - i * tencacleOffset, yRCell);
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
