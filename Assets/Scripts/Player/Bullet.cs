using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 2f;

    [field: SerializeField] public int damage { get; private set; }

    public bool Flying = false;

    private Vector3 startPosition;

    void OnEnable()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Flying)
            CheckDistance();
    }

    public void Fly(Vector2 direction)
    {
        Flying = true;
        gameObject.SetActive(true);
        rb.velocity = direction * speed;
    }

    private void CheckDistance()
    {
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxDistance)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        Flying = false;
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
