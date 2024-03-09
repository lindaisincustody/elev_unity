using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Player
    public event Action OnInteract    = delegate { };
    public event Action OnCancel      = delegate { };
    public event Action OnJump        = delegate { };
    public event Action OnFire        = delegate { };
    public event Action OnNext        = delegate { };
    // UI
    public event Action<Vector2> OnNavigate    = delegate { };
    public event Action OnSubmit = delegate { };
    public event Action OnUICancel = delegate { };
    public event Action OnInventory = delegate { };
    public Vector2 inputVector { get; private set; }

    Controls inputActions;

    private void Awake()
    {
        inputActions = new Controls();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.UI.Enable();

        inputActions.Player.Interact.performed    += Interact;
        inputActions.Player.Cancel.performed      += Cancel;
        inputActions.Player.Jump.performed        += Jump;
        inputActions.Player.Fire.performed        += Fire;
        inputActions.Player.Next.performed        += Next;

        inputActions.UI.Navigate.performed        += Navigate;
        inputActions.UI.Submit.performed          += Sumbit;
        inputActions.UI.Cancel.performed += UICancel;
        inputActions.UI.Inventory.performed += UIInventory;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    private void Update()
    {
        inputVector = inputActions.Player.Move.ReadValue<Vector2>();
    }

    private void Navigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnNavigate(inputActions.UI.Navigate.ReadValue<Vector2>());
        }
    }

    private void Sumbit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSubmit();
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInteract();
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCancel();
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJump();
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnFire();
        }
    }

    private void Next(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnNext();
        }
    }

    private void UICancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnUICancel();
        }
    }

    private void UIInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInventory();
        }
    }
}
