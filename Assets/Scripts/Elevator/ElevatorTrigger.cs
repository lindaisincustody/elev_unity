using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorButtonTrigger : Interactable
{
    [SerializeField] public UnityEvent OnElevatorGameTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerIsInTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        player.ShowInteractUI(playerIsInTrigger && ElevatorGameManager.Instance.HasPassenger);
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
        if (!playerIsInTrigger || !ElevatorGameManager.Instance.HasPassenger)
            return;

        base.HandleInteract();

        ElevatorGameManager.Instance.Play().Forget();

        OnElevatorGameTriggered?.Invoke();
    }
}
