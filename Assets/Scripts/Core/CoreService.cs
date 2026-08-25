using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class CoreService : MonoBehaviour
{
    public abstract UniTask Initialize();
}
