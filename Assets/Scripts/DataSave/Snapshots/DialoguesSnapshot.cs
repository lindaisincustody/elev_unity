using System;
using System.Collections.Generic;

[Serializable]
public class DialoguesSnapshot
{
    public List<string> Completed = new List<string>();

    public bool IsCompleted(string dialogueID)
    {
        return Completed.Contains(dialogueID);
    }
}
