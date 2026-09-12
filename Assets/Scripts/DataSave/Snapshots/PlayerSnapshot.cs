using System;

[Serializable]
public class PlayerSnapshot
{
    public int Gold;
    public int CurrentLevel;
    public int PoemsUsed;
    public bool TutorialComplete;

    public PlayerSnapshot()
    {
    }

    public PlayerSnapshot(int gold, int currentLevel, int poemsUsed, bool tutorialComplete)
    {
        Gold = gold;
        CurrentLevel = currentLevel;
        PoemsUsed = poemsUsed;
        TutorialComplete = tutorialComplete;
    }
}
