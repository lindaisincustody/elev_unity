using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scales : MonoBehaviour
{
    [SerializeField] NumberDisplayer ones;
    [SerializeField] NumberDisplayer tens;
    [SerializeField] NumberDisplayer hundreds;
    [SerializeField] NumberDisplayer thousands;
    [SerializeField] NumberDisplayer tenThousands;

    public void UpdateScales(float newWeight)
    {
        // Convert the weight to an integer (assuming weight is always non-negative)
        int weight = Mathf.FloorToInt(newWeight);

        // Extract individual digits from the weight
        int onesDigit = weight % 10;
        int tensDigit = (weight / 10) % 10;
        int hundredsDigit = (weight / 100) % 10;
        int thousandsDigit = (weight / 1000) % 10;
        int tenThousandsDigit = (weight / 10000) % 10;

        // Display each digit using the respective NumberDisplayer
        ones.ShowNumber(onesDigit);
        tens.ShowNumber(tensDigit);
        hundreds.ShowNumber(hundredsDigit);
        thousands.ShowNumber(thousandsDigit);
        tenThousands.ShowNumber(tenThousandsDigit);
    }
}
