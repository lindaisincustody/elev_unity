using System.Collections.Generic;
using UnityEngine;

public class RoomUnlocker : MonoBehaviour
{
    [SerializeField] private List<RoomId> rooms = new List<RoomId>();

    public void Unlock()
    {
        foreach (RoomId room in rooms)
            RoomManager.Instance.Unlock(room);
    }

    public void Lock()
    {
        foreach (RoomId room in rooms)
            RoomManager.Instance.Lock(room);
    }
}
