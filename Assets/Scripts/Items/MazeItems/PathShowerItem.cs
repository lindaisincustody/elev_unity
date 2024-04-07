using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazePathShowerItem", menuName = "Custom/Maze/PathShower")]
public class PathShowerItem : MiniGameItem
{
    public int PathShowerCountIncrease = 1;
    public override void OnGameStart()
    {
        ItemManager.instance.GetMazeManager.IncreasePathShowerAmount(PathShowerCountIncrease);
    }
}
