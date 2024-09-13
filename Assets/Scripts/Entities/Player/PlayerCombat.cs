using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    [SerializeField] Camera mainCamera;

    private int poolSize = 10; 

    private List<Bullet> bulletPool = new List<Bullet>();
    private GameObject poolHolder;

    private void Start()
    {
        InitializeBulletPool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SanityEffectHandler.IsPlayerInUnderworld)
            Shoot();
    }

    private void Shoot()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = (mousePosition - transform.position).normalized;

        Bullet newBullet = GetPooledBullet();
        if (newBullet == null)
        {
            newBullet = Instantiate(bullet, poolHolder.transform);
            newBullet.gameObject.SetActive(false);
            bulletPool.Add(newBullet);
        }

        newBullet.transform.position = transform.position;
        newBullet.Fly(direction);
    }

    private void InitializeBulletPool()
    {
        poolHolder = new GameObject("Bullets");
        bulletPool = new List<Bullet>();
        for (int i = 0; i < poolSize; i++)
        {
            Bullet newBullet = Instantiate(bullet, poolHolder.transform);
            newBullet.gameObject.SetActive(false);
            bulletPool.Add(newBullet);
        }
    }

    private Bullet GetPooledBullet()
    {
        foreach (Bullet bullet in bulletPool)
        {
            if (!bullet.Flying)
            {
                return bullet;
            }
        }
        return null;
    }
}
