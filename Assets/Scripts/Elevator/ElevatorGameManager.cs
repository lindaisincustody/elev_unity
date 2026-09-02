using Cysharp.Threading.Tasks;
using UnityEngine;

public class ElevatorGameManager : MonoBehaviour
{
    [Header("Passengers")] [SerializeField]
    private NPCData[] npcDataList;

    [Header("Settings")] [SerializeField] private int totalFloors = 6;
    [SerializeField] private int miniGameLevels = 3;
    [SerializeField] private int sanityCost = 50;

    public async UniTaskVoid Play()
    {
        ElevatorCanvas canvas = UIManager.Instance.Get<ElevatorCanvas>();

        bool completed = await canvas.Ride(CreatePassenger(), miniGameLevels);

        canvas.Close();

        if (!completed)
            return;

        SanityManager.Instance.DecreaseSanity(sanityCost);
    }

    private NPCData CreatePassenger()
    {
        NPCData passenger = Instantiate(npcDataList[Random.Range(0, npcDataList.Length)]);
        passenger.requestedFloor = Random.Range(2, totalFloors + 1);

        return passenger;
    }
}
