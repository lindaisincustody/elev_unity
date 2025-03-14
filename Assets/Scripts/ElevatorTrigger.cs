using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorButtonTrigger : Interactable
{
    [Header("Elevator Dependencies")] [SerializeField]
    private ElevatorManager elevatorManager;

    [SerializeField] private int floorToTrigger = 1; // Set the floor number for this button
    [SerializeField] public UnityEvent OnElevatorGameTriggered;

    [Header("UI Elements")] [SerializeField]
    private GameObject elevatorLeverGameObject;

    [SerializeField] private GameObject elevatorLevelsGameObject;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerIsInTrigger = true;
        player.ShowInteractUI(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerIsInTrigger = false;
        player.ShowInteractUI(false);
    }

    protected override void HandleInteract()
    {
        if (!playerIsInTrigger)
            return;

        base.HandleInteract();

        // Pop up the Elevator Lever and Elevator Levels game objects.
        if (elevatorLeverGameObject != null)
            elevatorLeverGameObject.SetActive(true);
        if (elevatorLevelsGameObject != null)
            elevatorLevelsGameObject.SetActive(true);

        // Trigger the NPC spawn for the elevator mini-game.
        elevatorManager.SpawnNPCPassenger();

        // Optionally, invoke any additional UnityEvents (sound, animation, etc.)
        OnElevatorGameTriggered?.Invoke();
    }
}