using UnityEngine;

public class Player : Entity
{
    [field: SerializeField] public Animator Animator { get; private set; }

    public static Player instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Double Player or singleton problem");

        instance = this;

        foreach (var component in components)
        {
            component.Init(this);
        }
    }
}
