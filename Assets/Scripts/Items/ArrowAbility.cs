using UnityEngine;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu(fileName = "ArrowAbility", menuName = "Custom/Ability/ArrowAbility")]
public class ArrowAbility : Ability
{
    [SerializeField] private GameObject arrowParticlePrefab;

    public override void Activate()
    {
        Debug.Log("Arrow ability activated.");
        OnActivate?.Invoke();
    }

    public override void Destroy()
    {
        OnCooldown?.Invoke();
    }

    public void SpawnArrowEffect(MonoBehaviour runner, Transform target)
    {
        if (arrowParticlePrefab == null)
        {
            Debug.LogWarning("ArrowAbility: arrowParticlePrefab is not assigned!");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("ArrowAbility: No main camera found!");
            return;
        }

        float distance = Mathf.Abs(cam.transform.position.z);
        Vector3 lowerLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, distance));
        Vector3 upperRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));

        Vector3 spawnPos = new Vector3(lowerLeft.x - 10f, target.position.y, 0);

        GameObject arrowInstance = Instantiate(arrowParticlePrefab, spawnPos, Quaternion.identity);

        Vector3 direction = (target.position - spawnPos).normalized;
        arrowInstance.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        runner.StartCoroutine(DestroyAfterTime(arrowInstance, 6f));
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}