using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int shotsPerCannon = 1;
    [SerializeField] Cannon[] cannons;
    [SerializeField] Animator anim;
    [SerializeField] NeutralityManager manager;

    void Start()
    {
        manager.gameStart += StartEnemy;
    }

    private void StartEnemy()
    {
        StartCoroutine(ChooseCannon());
    }

    private IEnumerator ChooseCannon()
    {
        if (cannons.Length == 0)
        {
            Debug.LogWarning("No cannons assigned to the enemy.");
            yield break;
        }

        int cannonIndex = Random.Range(0, cannons.Length);
        Cannon selectedCannon = cannons[cannonIndex];

        if (selectedCannon == null)
        {
            Debug.LogWarning("Selected cannon is null.");
            yield break;
        }

        // Assuming your cannon has a child (cannonBody) for rotation
        Transform cannonBody = selectedCannon.transform.GetChild(0);

        // Use the cannon's position and the cannonBody's rotation
        transform.position = selectedCannon.standingZone.position;
        transform.rotation = Quaternion.Euler(selectedCannon.transform.eulerAngles.x, cannonBody.eulerAngles.y, selectedCannon.transform.eulerAngles.z);

        anim.Play("FireCannon");
        selectedCannon.Shoot(shotsPerCannon);

        yield return new WaitForSeconds(5f);

        // Start the coroutine again
        StartCoroutine(ChooseCannon());
    }

    private void OnDestroy()
    {
        manager.gameStart -= StartEnemy;
    }
}
