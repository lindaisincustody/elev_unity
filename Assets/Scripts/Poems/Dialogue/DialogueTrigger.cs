using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : Interactable
{
    [SerializeField] private SaveId dialogueID;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] public UnityEvent OnComplete;

    private DialoguesSnapshot snapshot;

    protected override void Start()
    {
        base.Start();

        snapshot = SaveLoadService.Instance.Get<GeneralSaveFile>().DialoguesSnapshot;

        if (snapshot.IsCompleted(dialogueID.Value))
            Hide();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        playerIsInTrigger = true;
        player.ShowInteractUI(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        playerIsInTrigger = false;
        player.ShowInteractUI(false);
    }

    protected override void HandleInteract()
    {
        if (!playerIsInTrigger) return;

        base.HandleInteract();

        Trigger();
    }

    protected virtual void Trigger()
    {
        ActivateDialogue();
    }

    public virtual void Complete()
    {
        OnComplete?.Invoke();

        if (!snapshot.IsCompleted(dialogueID.Value))
        {
            snapshot.Completed.Add(dialogueID.Value);
            SaveLoadService.Instance.SaveProgress();
        }

        Hide();
    }

    protected virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ActivateDialogue()
    {
        UIManager.Instance.Get<DialogueController>().NextAction();
        UIManager.Instance.Get<DialogueController>().ActivateDialogue(dialogueData, this);
    }
}
