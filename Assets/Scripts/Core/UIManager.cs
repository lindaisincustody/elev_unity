using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private UIRoot rootPrefab;
    [SerializeField] private List<GameObject> spawnOnAwake = new List<GameObject>();
    [SerializeField] private List<GameObject> uiPrefabs = new List<GameObject>();

    private UIRoot root;
    private readonly Dictionary<Type, UnityEngine.Component> spawned = new Dictionary<Type, UnityEngine.Component>();


    private void Awake()
    {
        Instance = this;
        root = Instantiate(rootPrefab, transform, false);
        root.name = rootPrefab.name;

        foreach (GameObject prefab in spawnOnAwake)
            Spawn(prefab);
    }

    public T Get<T>() where T : UnityEngine.Component
    {
        Type type = typeof(T);

        if (spawned.TryGetValue(type, out UnityEngine.Component existing))
            return (T)existing;

        T component = Spawn(uiPrefabs.Find(p => p.GetComponent<T>())).GetComponent<T>();
        spawned[type] = component;
        return component;
    }

    private GameObject Spawn(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, root.Holder, false);
        instance.name = prefab.name;

        RectTransform rect = instance.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;

        foreach (MonoBehaviour component in instance.GetComponents<MonoBehaviour>())
            spawned[component.GetType()] = component;

        return instance;
    }
}
