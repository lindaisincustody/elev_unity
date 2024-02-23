using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;

public class CursorController : MonoBehaviour
{
    [SerializeField] InputManager playerInput;
    [SerializeField] RectTransform cursor;
    [SerializeField] RectTransform leftCursorHolder;
    [SerializeField] RectTransform rightCursorHolder;
    private UIElement[] UIElements;
    private UIElement activeUI;

    private int columns = 1;
    private int rows = 1;

    private bool isCursorAcitve = false;

    private void OnEnable()
    {
        playerInput.OnNavigate += OnNavigate;
        playerInput.OnSubmit += Sumbit;
    }

    private void OnDisable()
    {
        playerInput.OnNavigate -= OnNavigate;
        playerInput.OnSubmit -= Sumbit;
    }

    private void Sumbit()
    {
        if (activeUI == null)
            return;
        activeUI.elementAction?.Invoke();
    }

    private void OnNavigate(Vector2 value)
    {
        if (!isCursorAcitve)
            return;

        if (value == Vector2.left)
            NavigateLeft();
        else if (value == Vector2.right)
            NavigateRight();
        else if (value == Vector2.up)
            NavigateUp();
        else if (value == Vector2.down)
            NavigateDown();
    }

    private void NavigateLeft()
    {
        if (activeUI.col - 1 <= 0)
            return;

        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i].col == activeUI.col - 1 && UIElements[i].row == activeUI.row)
            {
                activeUI = UIElements[i];
                cursor.position = activeUI.rect.position;
                break;
            }
        }

        UpdateCursorSpacing();
    }
    private void NavigateRight()
    {
        if (activeUI.col + 1 > columns)
            return;

        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i].col == activeUI.col + 1 && UIElements[i].row == activeUI.row)
            {
                activeUI = UIElements[i];
                cursor.position = activeUI.rect.position;
                break;
            }
        }

        UpdateCursorSpacing();
    }
    private void NavigateUp()
    {
        if (activeUI.row - 1 <= 0)
            return;

        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i].row == activeUI.row - 1 && UIElements[i].col == activeUI.col)
            {
                activeUI = UIElements[i];
                cursor.position = activeUI.rect.position;
                break;
            }
        }

        UpdateCursorSpacing();
    }
    private void NavigateDown()
    {
        if (activeUI.row + 1 > rows)
            return;

        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i].row == activeUI.row + 1 && UIElements[i].col == activeUI.col)
            {
                activeUI = UIElements[i];
                cursor.position = activeUI.rect.position;
                break;
            }
        }

        UpdateCursorSpacing();
    }

    private void UpdateCursorSpacing()
    {
        leftCursorHolder.anchoredPosition = Vector2.zero;
        rightCursorHolder.anchoredPosition = Vector2.zero;
        leftCursorHolder.anchoredPosition -= new Vector2(activeUI.cursorSpace, leftCursorHolder.anchoredPosition.y);
        rightCursorHolder.anchoredPosition += new Vector2(activeUI.cursorSpace, leftCursorHolder.anchoredPosition.y);
    }

    public void ActivateCursor(UIElement[] newUIElements)
    {
        isCursorAcitve = true;
        activeUI = newUIElements[0];
        UIElements = newUIElements;
        for (int i = 0; i < newUIElements.Length; i++)
        {
            if (newUIElements[i].col > columns)
                columns = newUIElements[i].col;
            if (newUIElements[i].row > rows)
                rows = newUIElements[i].row;
        }

        cursor.gameObject.SetActive(true);
        cursor.position = activeUI.rect.position;

        UpdateCursorSpacing();
    }

    public void DeactivateCursor()
    {
        isCursorAcitve = false;
        cursor.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class UIElement
{
    public int col;
    public int row;
    public float cursorSpace;
    public RectTransform rect;
    public UnityEvent elementAction;
}
