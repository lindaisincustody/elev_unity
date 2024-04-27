using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialInteractable : Interactable
{
    [SerializeField] TutorialPopUp tutorialPopUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutorialPopUp.ActivatePopUp();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutorialPopUp.DeactivatePopUp();
        }
    }
}
