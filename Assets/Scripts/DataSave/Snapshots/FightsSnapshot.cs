using System;
using System.Collections.Generic;

[Serializable]
public class FightsSnapshot
{
    public List<FightState> Fights = new List<FightState>();

    public FightState GetFight(string fightId)
    {
        foreach (FightState fight in Fights)
        {
            if (fight.FightId == fightId)
                return fight;
        }

        return null;
    }
}

[Serializable]
public class FightState
{
    public string FightId;
    public List<int> RemainingEnemies = new List<int>();

    public FightState()
    {
    }

    public FightState(string fightId, List<int> remainingEnemies)
    {
        FightId = fightId;
        RemainingEnemies = remainingEnemies;
    }
}
