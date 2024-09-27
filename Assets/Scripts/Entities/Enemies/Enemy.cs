using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject underworldBody;
    [SerializeField] private GameObject overworldBody;

    [SerializeField] private EnemyMovement enemyMovement;

    public Vector2 minBound { get; set; }
    public Vector2 maxBound { get; set; }

    void Start()
    {
        SanityBar.instance.OnSanityChange += SanityChange;
        SanityChange(0);
        SetBounds();
    }

    private void SetBounds()
    {
        enemyMovement.minBound = minBound;
        enemyMovement.maxBound = maxBound;
    }

    private void SanityChange(int amount)
    {
        if (SanityEffectHandler.IsPlayerInUnderworld)
            ShowEnemy();
        else
            ShowSparkle();
    }

    private void ShowEnemy()
    {
        underworldBody.SetActive(true);
        overworldBody.SetActive(false);
    }

    private void ShowSparkle()
    {
        underworldBody.SetActive(false);
        overworldBody.SetActive(true);
    }
}
