using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Abiility", menuName = "Custom/Ability/BombAbility")]
public class BombAbility : Ability
{
    [Space, Header("Bomb Ability")]
    [SerializeField] private Bomb wavePrefab;
    [SerializeField] private float explosionDelay = 1f;
    [SerializeField] private int damage = 10;

    public override void Activate()
    {
        base.Activate();
        SpawnSplashEffect(Player.instance.transform, Player.instance.Get<PlayerMovement>().movement);
    }

    public void SpawnSplashEffect(Transform playerTransform, Vector2 playerLookRotation)
    {
        Vector2 dashDirection = playerLookRotation.normalized;

        Vector3 offset;
        if (dashDirection.x < 0)
        {
            offset = Vector2.left;
        }
        else
        {
            offset = Vector2.right;
        }

        Bomb newWave = Instantiate(wavePrefab, playerTransform.position + offset, Quaternion.identity);

        newWave.damage = damage;
        newWave.explosionDelay = explosionDelay;

        newWave.Init();
    }
}
