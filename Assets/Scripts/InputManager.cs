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
    public event Action OnShoot       = delegate { };
    // UI
    public event Action<Vector2> OnNavigate    = delegate { };
    public event Action OnSubmit = delegate { };
    public event Action OnUICancel = delegate { };
    public event Action OnInventory = delegate { };
    public event Action OnPoem = delegate { };
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

        inputActions.Player.Interact.started += Interact;
        inputActions.Player.Cancel.started += Cancel;
        inputActions.Player.Jump.started += Jump;
        inputActions.Player.Next.started += Next;
        inputActions.Player.Shoot.started += Shoot;

        inputActions.UI.Navigate.performed += Navigate;
        inputActions.UI.Submit.started += Sumbit;
        inputActions.UI.Cancel.started += UICancel;
        inputActions.UI.Inventory.started += UIInventory;
        inputActions.UI.Poem.started += UIPoem;
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
            // I dont know a better solution, but started does not fucking work with vector2
            if (inputActions.UI.Navigate.ReadValue<Vector2>() == Vector2.zero)
                return;
            OnNavigate(inputActions.UI.Navigate.ReadValue<Vector2>());
        }
    }

    private void Sumbit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSubmit();
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInteract();
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnCancel();
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnJump();
        }
    }

    private void Next(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnNext();
        }
    }
    
    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnShoot();
        }
    }

    private void UICancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnUICancel();
        }
    }

    private void UIInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInventory();
        }
    }

    private void UIPoem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnPoem();
        }
    }
}
