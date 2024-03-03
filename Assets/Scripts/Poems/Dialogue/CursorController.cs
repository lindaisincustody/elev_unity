using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [SerializeField] InputManager playerInput;
    [SerializeField] RectTransform cursor;
    [SerializeField] RectTransform leftCursorHolder;
    [SerializeField] RectTransform rightCursorHolder;
    [SerializeField] Image leftCursorFiller;
    [SerializeField] Image rightCursorFiller;
    [SerializeField] AttributeParticles particleManager;
    [SerializeField] SpriteAdjuster spriteMarker;
    [Space]
    public bool issMinigameCursor = false;
    private UIElement[] UIElements;
    private UIElement activeUI;
    private Action onSubmitAction;

    private int columns = 1;
    private int rows = 1;
    private float poemWordWidth = 140f;
    
    private bool isCursorAcitve = false;
    private bool isCentered = true;

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
        // Calls a callback to poem/dialogue
        onSubmitAction.Invoke();
        // Loads New Scene
        activeUI.elementAction?.Invoke();
        onSubmitAction = null;
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
        if (issMinigameCursor)
            UpdateFillerColor();
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
        if (issMinigameCursor)
            UpdateFillerColor();
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
        if (issMinigameCursor)
            UpdateFillerColor();
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
        if (issMinigameCursor)
            UpdateFillerColor();
    }

    private void UpdateCursorSpacing()
    {
        if (isCentered)
        {
            leftCursorHolder.anchoredPosition = Vector2.zero;
            rightCursorHolder.anchoredPosition = Vector2.zero;
            leftCursorHolder.anchoredPosition -= new Vector2(activeUI.cursorSpace, leftCursorHolder.anchoredPosition.y);
            rightCursorHolder.anchoredPosition += new Vector2(activeUI.cursorSpace, leftCursorHolder.anchoredPosition.y);
        }
        else
        {
            leftCursorHolder.anchoredPosition = Vector2.zero;
            rightCursorHolder.anchoredPosition = Vector2.zero;
            leftCursorHolder.anchoredPosition -= new Vector2(poemWordWidth, leftCursorHolder.anchoredPosition.y);
            rightCursorHolder.anchoredPosition -= new Vector2((poemWordWidth - activeUI.cursorSpace), leftCursorHolder.anchoredPosition.y);
        }
    }

    public void ActivateCursor(UIElement[] newUIElements, Action newAction)
    {
        onSubmitAction = newAction;
        isCursorAcitve = true;
        activeUI = newUIElements[0];
        UIElements = newUIElements;
        isCentered = activeUI.isCentered;
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
        if (issMinigameCursor)
            UpdateFillerColor();
    }

    public void DeactivateCursor()
    {
        activeUI = null;
        isCursorAcitve = false;
        cursor.gameObject.SetActive(false);
        if (issMinigameCursor)
            ResetHighlights();
    }

    private void ResetHighlights()
    {
        spriteMarker.ResetScales();
    }

    public void UpdateFillerColor()
    {
        if (activeUI.row == 1 && activeUI.col == 1)
        {
            leftCursorFiller.color = Color.red;
            rightCursorFiller.color = Color.red;
            particleManager.ActivatePSS(Attribute.Strength);
            spriteMarker.markAttribute(Attribute.Strength);
        }
        else if (activeUI.row == 1 && activeUI.col == 2)
        {
            leftCursorFiller.color = Color.green;
            rightCursorFiller.color = Color.green;
            particleManager.ActivatePSS(Attribute.Coordination);
            spriteMarker.markAttribute(Attribute.Coordination);
        }
        else if (activeUI.row == 2 && activeUI.col == 1)
        {
            leftCursorFiller.color = Color.blue;
            rightCursorFiller.color = Color.blue;
            particleManager.ActivatePSS(Attribute.Intelligence);
            spriteMarker.markAttribute(Attribute.Intelligence);
        }
        else if (activeUI.row == 2 && activeUI.col == 2)
        {
            leftCursorFiller.color = Color.grey;
            rightCursorFiller.color = Color.grey;
            particleManager.ActivatePSS(Attribute.Neutrality);
            spriteMarker.markAttribute(Attribute.Neutrality);
        }
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
    [System.NonSerialized] public bool isCentered = true;
}
