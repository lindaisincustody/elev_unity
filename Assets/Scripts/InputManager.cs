using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action OnInteract    = delegate { };
    public event Action OnCancel      = delegate { };
    public event Action OnJump        = delegate { };
    public event Action OnFire        = delegate { };
    public event Action OnNext        = delegate { };
    public Vector2 inputVector { get; private set; }

    Controls inputActions;

    private void Awake()
    {
        inputActions = new Controls();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed    += Interact;
        inputActions.Player.Cancel.performed      += Cancel;
        inputActions.Player.Jump.performed        += Jump;
        inputActions.Player.Fire.performed        += Fire;
        inputActions.Player.Next.performed        += Next;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        inputVector = inputActions.Player.Move.ReadValue<Vector2>();
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

}
