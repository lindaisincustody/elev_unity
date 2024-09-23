using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    [SerializeField] GameObject chinchillaPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] Text combatModeText;

    private int poolSize = 10;
    private List<Bullet> bulletPool = new List<Bullet>();
    private GameObject poolHolder;

    private Player player;
    private InputManager inputManager;

    // Enum for combat modes
    private CombatMode currentMode = CombatMode.Bullet;
    private CombatMode[] combatModes;
    public enum CombatMode
    {
        Bullet,
        Chinchilla
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        inputManager = player.GetInputManager;
        inputManager.OnShoot += Shoot;

        combatModes = (CombatMode[])System.Enum.GetValues(typeof(CombatMode));
    }

    private void Start()
    {
        InitializeBulletPool();
        UpdateCombatModeUI();
    }

    private void Update()
    {
        HandleModeSwitching();
    }

    private void Shoot()
    {
        if (!SanityEffectHandler.IsPlayerInUnderworld)
            return;

        switch (currentMode)
        {
            case CombatMode.Bullet:
                ShootBullet();
                break;

            case CombatMode.Chinchilla:
                SummonChinchilla();
                break;
        }
    }

    private void ShootBullet()
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

    private void HandleModeSwitching()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            // Scroll up (next mode)
            int nextIndex = (System.Array.IndexOf(combatModes, currentMode) + 1) % combatModes.Length;
            currentMode = combatModes[nextIndex];
        }
        else if (scroll < 0f)
        {
            // Scroll down (previous mode)
            int previousIndex = (System.Array.IndexOf(combatModes, currentMode) - 1 + combatModes.Length) % combatModes.Length;
            currentMode = combatModes[previousIndex];
        }

        UpdateCombatModeUI();
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
    private void UpdateCombatModeUI()
    {
        combatModeText.text = "Mode: " + currentMode.ToString(); // Update the UI with the current mode
    }

    private void OnDestroy()
    {
        inputManager.OnShoot -= Shoot;
    }
    private void SummonChinchilla()
    {
        if (chinchillaPrefab != null)
        {
            Instantiate(chinchillaPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Chinchilla prefab not assigned!");
        }
    }
}
