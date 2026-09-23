using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    [Serializable]
    private class Spawn
    {
        public Npc prefab;
        public Transform point;
    }

    [SerializeField] private List<Spawn> spawns = new List<Spawn>();

    [Header("Area")]
    [SerializeField] private Transform areaMin;
    [SerializeField] private Transform areaMax;

    private void Start()
    {
        foreach (Spawn spawn in spawns)
        {
            SpawnNpc(spawn);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((areaMin.position + areaMax.position) * 0.5f, areaMax.position - areaMin.position);
    }

    private void SpawnNpc(Spawn spawn)
    {
        Npc npc = Instantiate(spawn.prefab, spawn.point.position, Quaternion.identity);
        npc.name = spawn.prefab.name;
        npc.Get<NpcMovement>().SetArea(areaMin.position, areaMax.position);

        NpcManager.Instance.RegisterNpc(npc);
    }
}
