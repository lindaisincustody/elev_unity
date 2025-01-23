using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    [SerializeField] Bomb bomb;
    [SerializeField] GameObject chinchillaPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] TextMeshProUGUI combatModeText;

    private int poolSize = 10;
    private List<Bullet> bulletPool = new List<Bullet>();
    private List<Bomb> bombPool = new List<Bomb>();

    private int activeBombCount = 0;
    private const int maxActiveBombs = 3;

    [SerializeField] private float meleeAttackRange = 2f; 
    [SerializeField] private int meleeDamage = 5;
    private float meleeCooldown = 0.7f;
    private float nextMeleeTime = 0f;

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
        if (!SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
            return;

        switch (currentMode)
        {
            case CombatMode.Bullet:
                //ShootBullet();
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

    public void ShootHomingBullet()
    {
        Bullet homingBullet = GetPooledBullet();
        if (homingBullet == null)
        {
            homingBullet = Instantiate(bullet, poolHolder.transform);
            bulletPool.Add(homingBullet);
        }

        homingBullet.transform.position = transform.position;
        homingBullet.gameObject.SetActive(true);
        homingBullet.Fly(Vector2.zero, FindClosestEnemy()); // Pass the closest enemy as target
    }


    private Transform FindClosestEnemy()
    {
        float minDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Enemy enemy in EnemyManager.Instance.GetAllEnemies())
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
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
        // Check if the current time is greater than or equal to the next allowed melee time
        if (Time.time < nextMeleeTime)
        {
            return; // Exit if melee attack is on cooldown
        }

        // Update the next allowed melee time
        nextMeleeTime = Time.time + meleeCooldown;

        player.GetComponent<PlayerMovement>().isAttacking = true;

        // Get the mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate the direction vector from the player to the mouse
        Vector2 attackDirection = (mousePosition - transform.position).normalized;

        // Determine the attack direction and set the corresponding animation
        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            if (attackDirection.x > 0) // Right
            {
                animator.SetTrigger("MeleeRight");
            }
            else // Left
            {
                animator.SetTrigger("MeleeLeft");
            }
        }
        else
        {
            if (attackDirection.y > 0) // Back
            {
                animator.SetTrigger("MeleeBack");
            }
            else // Front
            {
                animator.SetTrigger("MeleeFront");
            }
        }

        // Start the delayed hit detection
        StartCoroutine(DelayedMeleeHitDetection(0.2f)); // Adjust delay to match the animation's strike frame
    }

    private IEnumerator DelayedMeleeHitDetection(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Detect enemies within melee range
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange);

        foreach (Collider2D collider in hitObjects)
        {
            // Check if the object has an EnemyHealth component
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            Enemy enemyComponent = collider.GetComponent<Enemy>();

            if (enemyHealth != null && enemyHealth.currentHealth > 0 && enemyComponent.activeSymbols.Count == 0)
            {
                enemyHealth.TakeDamage(meleeDamage);
            }
        }

        // Reset attack after a short delay
        StartCoroutine(ResetMeleeAnimation());
    }

    private IEnumerator ResetMeleeAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        // Reset all melee animation triggers
        animator.ResetTrigger("MeleeRight");
        animator.ResetTrigger("MeleeLeft");
        animator.ResetTrigger("MeleeBack");
        animator.ResetTrigger("MeleeFront");

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
