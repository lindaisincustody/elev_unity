using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawingConfig", menuName = "Custom/Drawing Config")]
public class DrawingConfig : ScriptableObject
{
    [Serializable]
    public class ModeEntry
    {
        public DrawingMode mode;
        public LineRenderer linePrefab;
        public Sprite sprite;
    }

    [SerializeField] private List<ModeEntry> modes = new List<ModeEntry>();

    public IReadOnlyList<ModeEntry> Modes => modes;

    public ModeEntry For(DrawingMode mode)
    {
        return modes.Find(entry => entry.mode == mode);
    }

    public Sprite SpriteFor(DrawingMode mode)
    {
        return For(mode).sprite;
    }
}
