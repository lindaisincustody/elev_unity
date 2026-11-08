using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolTemplates", menuName = "Custom/Symbol Templates")]
public class SymbolTemplates : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string label;
        public Vector2[] cloud;
    }

    [field: SerializeField] public List<Entry> Entries { get; private set; } = new List<Entry>();

    public PointCloudRecognizer CreateRecognizer()
    {
        PointCloudRecognizer recognizer = new PointCloudRecognizer();

        Entry[] active = Entries.Where(entry => SymbolRecognizer.Active.Contains(entry.label)).ToArray();
        for (int i = 0; i < active.Length; i++)
            recognizer.Add(active[i].label, active[i].cloud, i);

        return recognizer;
    }
}
