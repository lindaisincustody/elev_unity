using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpActivation : MonoBehaviour
{

    private CloneManager cloneManager;
    // Start is called before the first frame update
    private void Awake()
    {
        cloneManager = FindObjectOfType<CloneManager>(); // Find the CloneManager in the scene
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PowerUpDuration());
            gameObject.SetActive(false); // Deactivates the power-up object
        }
    }

    IEnumerator PowerUpDuration()
    {
        cloneManager.SpawnClones();
        yield return new WaitForSeconds(5);
        cloneManager.ClearClones();
    }
}
