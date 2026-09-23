using System;
using System.Collections.Generic;

[Serializable]
public class PickupsSnapshot
{
    public List<string> Collected = new List<string>();

    public bool IsCollected(string pickupId)
    {
        return Collected.Contains(pickupId);
    }
}
