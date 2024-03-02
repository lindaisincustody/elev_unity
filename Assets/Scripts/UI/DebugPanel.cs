using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] AttributesDisplayer attributesDisplayer;
    [SerializeField] GoldMultiplierDisplayer goldMultiplierDisplayer;
    private static DebugPanel instance;

    public static DebugPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DebugPanel>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DebugPanel");
                    instance = obj.AddComponent<DebugPanel>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateAttributes()
    {
        attributesDisplayer.UpdateText();
    }

    public void UpdateGoldMultiplier()
    {
        goldMultiplierDisplayer.UpdateText();
    }
}
