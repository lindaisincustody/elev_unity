using UnityEngine;

[CreateAssetMenu(fileName = "ReferenceConfig", menuName = "Custom/ReferenceConfig")]
public class ReferenceConfig : ScriptableObject
{
    [field: SerializeField] public PickUp PickUp { get; private set; }
}
