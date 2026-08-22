using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Abiility", menuName = "Custom/Ability/WaveAbility")]
public class WaveAbility : Ability
{
    [Space, Header("Wave Ability")]
    [SerializeField] private WaveBullet wavePrefab;
    [SerializeField] private float duration = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float pushForce = 0.6f;

    public override void Activate()
    {
        base.Activate();
        SpawnSplashEffect(Player.instance.transform);
    }

    public void SpawnSplashEffect(Transform playerTransform)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = playerTransform.position.z;
        Vector2 toMouse = mouseWorld - playerTransform.position;
        Vector2 dir = (mouseWorld - playerTransform.position).normalized;

        Vector3 startPos = playerTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.Euler(0, 0, angle);
        WaveBullet newWave = Instantiate(wavePrefab, startPos, rot);

        newWave.damage = damage;
        newWave.Force = pushForce;

        newWave.Fly(dir, duration);

        CoroutineRunner.RunCoroutine(DestroyAfterCooldown(2f));
    }

    private IEnumerator DestroyAfterCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy();
    }
}
