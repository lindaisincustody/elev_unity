using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public void ActivateDarkWorld()
    {
        SanityBar.instance.SanityToMin();
    }
}
