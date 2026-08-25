using System;

[Serializable]
public class PlayerSnapshot
{
    public int Gold;
    public int CurrentLevel;
    public int PoemsUsed;
    public bool TutorialComplete;
    public string LastFightId;

    public PlayerSnapshot()
    {
    }

    public PlayerSnapshot(int gold, int currentLevel, int poemsUsed, bool tutorialComplete, string lastFightId)
    {
        Gold = gold;
        CurrentLevel = currentLevel;
        PoemsUsed = poemsUsed;
        TutorialComplete = tutorialComplete;
        LastFightId = lastFightId;
    }
}
