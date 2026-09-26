using UnityEngine;
using UnityEngine.Playables;

public class DirectorCinematicTrigger : DialogueTrigger
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool disablePlayerInput;

    protected override void Trigger()
    {
        OnStart?.Invoke();

        if (disablePlayerInput)
            player.LockMovement(this);

        director.stopped += OnPlaybackStopped;
        director.Play();
    }

    private void OnPlaybackStopped(PlayableDirector aDirector)
    {
        if (aDirector != director) return;

        director.stopped -= OnPlaybackStopped;

        if (disablePlayerInput)
            player.UnlockMovement(this);

        Complete();
    }
}
