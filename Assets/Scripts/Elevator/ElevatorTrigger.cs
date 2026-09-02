using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorButtonTrigger : Interactable
{
    [SerializeField] private ElevatorGameManager elevatorGame;
    [SerializeField] public UnityEvent OnElevatorGameTriggered;

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

        elevatorGame.Play().Forget();

        OnElevatorGameTriggered?.Invoke();
    }
}
