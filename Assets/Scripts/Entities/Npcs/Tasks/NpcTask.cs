using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class NpcTask : ScriptableObject
{
    public abstract UniTask Run(NpcContext context, CancellationToken token);
}
