using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private Collectable collectableObj;
    public Transform collectablesHolder;
    [System.NonSerialized] public List<Collectable> collectables = new List<Collectable>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnCollectibles(5);
    }

    private void SpawnCollectibles(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            int x = Random.Range(-15, 15);
            int y = Random.Range(-9, 0);
            Collectable newCollectable = Instantiate(collectableObj, new Vector2(x, y), Quaternion.identity, collectablesHolder);
            collectables.Add(newCollectable);
        }
    }
}
