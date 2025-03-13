using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLetter : MonoBehaviour
{
    [field: SerializeField] public TMP_Text Letter { get; private set; }

    public void Show()
    {
        Letter.alpha = 1f;
    }

    public void Hide()
    {
        Letter.alpha = 0f;
    }
}
