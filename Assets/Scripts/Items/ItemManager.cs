using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] MazeManager _mazeManager;
    public static ItemManager instance;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        var items = ItemsInventory.Instance.GetAllItems();

        foreach (var item in items)
        {
            item.item.OnGameStart();
        }

        _mazeManager.StartMaze();
    }

    public MazeManager GetMazeManager => _mazeManager;
}
