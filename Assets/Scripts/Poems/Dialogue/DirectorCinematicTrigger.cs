using UnityEngine;
using UnityEngine.Playables;

public class DirectorCinematicTrigger : DialogueTrigger
{
    [SerializeField] private PlayableDirector director;

    protected override void Trigger()
    {
        director.stopped += OnPlaybackStopped;
        director.Play();
    }

    private void OnPlaybackStopped(PlayableDirector aDirector)
    {
        if (aDirector != director) return;

        director.stopped -= OnPlaybackStopped;

        ActivateDialogueInstantly();
    }
}
