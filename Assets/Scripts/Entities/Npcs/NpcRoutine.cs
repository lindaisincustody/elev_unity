using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Npc/Routine")]
public class NpcRoutine : ScriptableObject
{
    public List<NpcTask> tasks = new List<NpcTask>();
    public bool loop = true;
}
