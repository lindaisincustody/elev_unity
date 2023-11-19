using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform heart;

    void Start()
    {
        // Start the coroutine to instantiate projectiles
        StartCoroutine(InstantiateProjectiles());
    }

    IEnumerator InstantiateProjectiles()
    {
        while (true)
        {
            // Instantiate projectile
            GameObject projectile = Instantiate(projectilePrefab, GetSpawnPosition(Vector2.left), Quaternion.identity);
            projectile.GetComponent<Projectile>().target = heart;

            // Wait for the projectile to be destroyed before instantiating the next one
            yield return new WaitUntil(() => projectile == null);

            // Repeat for the projectile from the other side
            projectile = Instantiate(projectilePrefab, GetSpawnPosition(Vector2.right), Quaternion.identity);
            projectile.GetComponent<Projectile>().target = heart;

            yield return new WaitUntil(() => projectile == null);

            // Add a delay if needed before instantiating the next projectile
            yield return new WaitForSeconds(1f);
        }
    }

    Vector2 GetSpawnPosition(Vector2 direction)
    {
        float spawnX = (direction == Vector2.left) ? -10f : 10f;
        float spawnY = Random.Range(-5f, 5f);
        return new Vector2(spawnX, spawnY);
    }
}
