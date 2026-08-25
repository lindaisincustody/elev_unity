using System;

[Serializable]
public class GeneralSaveFile : ISaveFile
{
    public int SchemeVersion;
    public SceneSnapshot SceneSnapshot = new SceneSnapshot();
    public PlayerSnapshot PlayerSnapshot = new PlayerSnapshot();
    public InventorySnapshot InventorySnapshot = new InventorySnapshot();
    public AbilitiesSnapshot AbilitiesSnapshot = new AbilitiesSnapshot();
}
