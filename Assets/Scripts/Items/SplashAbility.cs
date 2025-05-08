using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplashAbility", menuName = "Custom/Ability/SplashAbility")]
public class SplashAbility : Ability
{
    [Header("Prefabs & Timings")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject splashPrefab;      
    [SerializeField] private float riseHeight = 2f;  
    [SerializeField] private float fallDuration = 0.7f;  
    [SerializeField] private float cooldownDuration = 2f;  

    [Header("Hop Settings")]
    [SerializeField] private float peakHeight = 0.5f;  
    [SerializeField] private float ascendDuration = 0.2f; 

    public override void Activate()
    {
        Debug.Log("Splash ability activated.");
        OnActivate?.Invoke();
    }

    public override void Destroy()
    {
        Debug.Log("Splash ability on cooldown.");
        OnCooldown?.Invoke();
    }

    public void SpawnSplashEffect(MonoBehaviour runner, Transform playerTransform)
    {
        if (bubblePrefab == null || splashPrefab == null)
        {
            Debug.LogWarning("SplashAbility: Prefabs not assigned!");
            return;
        }

        Vector3 startPos = playerTransform.position + Vector3.up * riseHeight;
        float groundY = playerTransform.position.y - 1.5f;

        GameObject bubble = Instantiate(bubblePrefab, startPos, Quaternion.identity);

        var seq = DOTween.Sequence();


        seq.Append(
            bubble.transform
                  .DOMoveY(startPos.y + peakHeight, ascendDuration)
                  .SetEase(Ease.OutQuad)
        );


        seq.Append(
            bubble.transform
                  .DOMoveY(groundY, fallDuration)
                  .SetEase(Ease.InQuad)
                  .OnComplete(() =>
                  {

                      Instantiate(
    splashPrefab,
    new Vector3(startPos.x, groundY, 0),
    Quaternion.identity
);
                      Destroy(bubble);
                  })
        );

        float totalAnimTime = ascendDuration + fallDuration + 0.1f;
        runner.StartCoroutine(DestroyAfterCooldown(totalAnimTime));
    }

    private IEnumerator DestroyAfterCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy();
    }
}