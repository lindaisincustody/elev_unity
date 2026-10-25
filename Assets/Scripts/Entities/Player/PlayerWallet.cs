using UnityEngine;

public class PlayerWallet : Component
{
    private readonly Gold gold = new Gold();
    private GeneralSaveFile saveFile;

    private void Awake()
    {
        saveFile = SaveLoadService.Instance.Get<GeneralSaveFile>();
        gold.AddGold(saveFile.PlayerSnapshot.Gold);
    }

    private void OnDestroy()
    {
        gold.Dispose();
    }

    public void AddGold(int goldAmount)
    {
        gold.AddGold(goldAmount);

        saveFile.PlayerSnapshot.Gold = gold.GetGold();
        SaveLoadService.Instance.SaveProgress();
    }

    public int GetGold()
    {
        return gold.GetGold();
    }

    public void AddMultiplier(Attribute attribute, float multiplier)
    {
        gold.AddMultiplier(attribute, multiplier);

        SaveLoadService.Instance.SaveProgress();
    }

    public float GetMultiplier(Attribute attribute)
    {
        return gold.GetMultiplier(attribute);
    }
}
