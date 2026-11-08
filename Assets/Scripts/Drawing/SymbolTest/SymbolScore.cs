using System.Collections.Generic;
using System.Linq;

public enum SymbolEngine { Model, Templates, Combined }

public class SymbolScore
{
    private readonly Dictionary<int, int> confusions = new Dictionary<int, int>();

    public int Attempts { get; private set; }
    public int Hits { get; private set; }
    public int Rejected { get; private set; }

    public void Record(int target, int guess, bool accepted)
    {
        Attempts++;

        if (!accepted)
            Rejected++;
        else if (guess == target)
            Hits++;
        else
            confusions[guess] = confusions.TryGetValue(guess, out int count) ? count + 1 : 1;
    }

    public void Clear()
    {
        Attempts = 0;
        Hits = 0;
        Rejected = 0;
        confusions.Clear();
    }

    public IEnumerable<KeyValuePair<int, int>> Confusions()
    {
        return confusions.OrderByDescending(entry => entry.Value);
    }
}
