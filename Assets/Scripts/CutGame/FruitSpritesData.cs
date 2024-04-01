using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Custom/FruitImage", menuName = "Custom/Fruit Images")]
public class FruitSpritesData : ScriptableObject
{
    public FruitObject[] fruit;
}

[System.Serializable]
public class FruitObject
{
    public Sprite Image;
    public GameObject Prefab;
}
