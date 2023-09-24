using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoemTrigger : MonoBehaviour
{
    [SerializeField] private WordData wordsData;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PoemMenuController.instance.OpenPoemBook(wordsData);
        }
    }
}
