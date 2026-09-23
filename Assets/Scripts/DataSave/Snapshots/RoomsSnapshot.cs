using System;
using System.Collections.Generic;

[Serializable]
public class RoomsSnapshot
{
    public bool Seeded;
    public List<RoomId> Unlocked = new List<RoomId>();
}
