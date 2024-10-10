using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    [SerializeField] Bomb bomb;
    [SerializeField] GameObject chinchillaPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] Text combatModeText;

    private int poolSize = 10;
    private List<Bullet> bulletPool = new List<Bullet>();
    private List<Bomb> bombPool = new List<Bomb>();

    private int activeBombCount = 0;
    private const int maxActiveBombs = 3;

    [SerializeField] private float meleeAttackRange = 2f; 
    [SerializeField] private int meleeDamage = 20; 

    private GameObject poolHolder;

    private Player player;
    private Animator animator;
    private InputManager inputManager;

    private CombatMode currentMode = CombatMode.Bullet;
    private CombatMode[] combatModes;

    public enum CombatMode
    {
        Bullet,
        Chinchilla,
        Bomb,
        Melee
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        inputManager = player.GetInputManager;
        inputManager.OnShoot += Shoot;

        combatModes = (CombatMode[])System.Enum.GetValues(typeof(CombatMode));
    }

    private void Start()
    {
        InitializeBulletPool();
        InitializeBombPool();
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
            case CombatMode.Bomb:
                ShootBomb();
                break;
            case CombatMode.Melee:
                PerformMeleeAttack(); 
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

    private void ShootBomb()
    {
        if (activeBombCount >= maxActiveBombs)
        {
            return;
        }

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = (mousePosition - transform.position).normalized;

        Bomb newBomb = GetPooledBomb();
        if (newBomb == null)
        {
            newBomb = Instantiate(bomb, poolHolder.transform);
            newBomb.gameObject.SetActive(false);
            bombPool.Add(newBomb);
        }

        newBomb.transform.position = transform.position;
        newBomb.Fly(direction);
        activeBombCount++;

        newBomb.OnBombExploded -= HandleBombExploded;
        newBomb.OnBombExploded += HandleBombExploded;
    }

    private void PerformMeleeAttack()
    {
        player.GetComponent<PlayerMovement>().isAttacking = true;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = (mousePosition - transform.position).normalized;

        if (direction.x > 0)
        {
            animator.SetBool("MeleeRight", true);
        }
        else
        {
            animator.SetBool("MeleeLeft", true);
        }

        StartCoroutine(ResetMeleeAnimation());
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(meleeDamage);
            }
        }
    }

    private IEnumerator ResetMeleeAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        animator.SetBool("MeleeRight", false);
        animator.SetBool("MeleeLeft", false);

        player.GetComponent<PlayerMovement>().isAttacking = false;
    }


    private void HandleBombExploded()
    {
        activeBombCount--;
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

    private Bomb GetPooledBomb()
    {
        foreach (Bomb bomb in bombPool)
        {
            if (!bomb.Flying && bomb.HasExploded)
            {
                return bomb;
            }
        }
        return null;
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

    private void InitializeBombPool()
    {
        poolHolder = new GameObject("Bombs");
        bombPool = new List<Bomb>();
        for (int i = 0; i < poolSize; i++)
        {
            Bomb newBomb = Instantiate(bomb, poolHolder.transform);
            newBomb.gameObject.SetActive(false);
            bombPool.Add(newBomb);
        }
    }

    private void HandleModeSwitching()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            int nextIndex = (System.Array.IndexOf(combatModes, currentMode) + 1) % combatModes.Length;
            currentMode = combatModes[nextIndex];
        }
        else if (scroll < 0f)
        {
            int previousIndex = (System.Array.IndexOf(combatModes, currentMode) - 1 + combatModes.Length) % combatModes.Length;
            currentMode = combatModes[previousIndex];
        }

        UpdateCombatModeUI();
    }

    private void UpdateCombatModeUI()
    {
        combatModeText.text = "Mode: " + currentMode.ToString();
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
