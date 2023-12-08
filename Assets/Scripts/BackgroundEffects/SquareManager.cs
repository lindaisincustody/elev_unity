using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour
{
    private List<Transform> squares = new List<Transform>();

    // Update is called once per frame
    void Update()
    {
        foreach (var sq in squares)
        {
            Vector3 directionAwayFromPlayer = (sq.position - transform.position).normalized;
            sq.position += directionAwayFromPlayer * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        squares.Add(collision.gameObject.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        squares.Remove(collision.gameObject.transform);
    }
}
