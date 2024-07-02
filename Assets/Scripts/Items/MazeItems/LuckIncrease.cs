using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeLuck", menuName = "Custom/Maze/MazeLuck")]
public class LuckIncrease : Item
{
    public float luckIncrease = 0;

    public override void OnGameStart()
    {
        ItemManager.instance.GetMazeManager.IncreaseLuck(luckIncrease);
    }

    public override void UseItem()
    {
         // No Effect On Use
    }
}
