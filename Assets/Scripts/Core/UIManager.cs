using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public class UIEntry
    {
        public GameObject prefab;
        public bool stackable;
        public int sortOrder;
    }

    public static UIManager Instance { get; private set; }

    [SerializeField] private UIRoot rootPrefab;
    [SerializeField] private List<UIEntry> spawnOnAwake = new List<UIEntry>();
    [SerializeField] private List<UIEntry> uiPrefabs = new List<UIEntry>();

    private UIRoot root;
    private readonly Dictionary<Type, UnityEngine.Component> spawned = new Dictionary<Type, UnityEngine.Component>();
    private readonly Dictionary<Type, bool> stackable = new Dictionary<Type, bool>();
    private readonly List<UnityEngine.Component> openPanels = new List<UnityEngine.Component>();


    private void Awake()
    {
        Instance = this;
        root = Instantiate(rootPrefab, transform, false);
        root.name = rootPrefab.name;

        foreach (UIEntry entry in spawnOnAwake)
            Spawn(entry);
    }

    public T Get<T>() where T : UnityEngine.Component
    {
        Type type = typeof(T);

        if (spawned.TryGetValue(type, out UnityEngine.Component existing))
            return (T)existing;

        T component = Spawn(uiPrefabs.Find(e => e.prefab.GetComponent<T>())).GetComponent<T>();
        spawned[type] = component;
        return component;
    }

    public bool RequestOpen(UnityEngine.Component ui)
    {
        if (openPanels.Contains(ui))
            return true;

        foreach (UnityEngine.Component open in openPanels)
        {
            if (!stackable[open.GetType()])
                return false;
        }

        openPanels.Add(ui);
        return true;
    }

    public void NotifyClosed(UnityEngine.Component ui)
    {
        openPanels.Remove(ui);
    }

    public bool IsOpen(UnityEngine.Component ui)
    {
        return openPanels.Contains(ui);
    }

    private GameObject Spawn(UIEntry entry)
    {
        GameObject instance = Instantiate(entry.prefab, root.Holder, false);
        instance.name = entry.prefab.name;

        RectTransform rect = instance.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;

        Canvas canvas = instance.GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = entry.sortOrder;

        foreach (MonoBehaviour component in instance.GetComponents<MonoBehaviour>())
        {
            spawned[component.GetType()] = component;
            stackable[component.GetType()] = entry.stackable;
        }

        return instance;
    }
}
