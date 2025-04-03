using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerCombat : Component
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI combatModeText;

    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private int meleeDamage = 5;
    [SerializeField] private float meleeCooldown = 0.7f;
    private float nextMeleeTime = 0f;

    private Player player;
    private Animator animator;
    private InputManager inputManager;

    public Enemy GetNearestEnemy()
    {
        Enemy nearestEnemy = null;
        float shortestDistance = float.MaxValue;
        Vector3 currentPosition = transform.position;

        foreach (Enemy enemy in EnemyManager.Instance.GetAllEnemies())
        {
            if (enemy == null) continue;
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        inputManager = player.GetInputManager;
        inputManager.OnShoot += Shoot;
    }

    private void Start()
    {
        if (combatModeText != null)
        {
            combatModeText.text = ""; //mode melee
        }
    }

    private void Update()
    {
    }

    private void Shoot()
    {
        if (!SanityBar.instance.sanityEffectHandler.IsPlayerInUnderworld)
            return;

        PerformMeleeAttack();
    }

    private void PerformMeleeAttack()
    {
        if (Time.time < nextMeleeTime)
            return;

        nextMeleeTime = Time.time + meleeCooldown;
        player.GetComponent<PlayerMovement>().isAttacking = true;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 attackDirection = (mousePosition - transform.position).normalized;

        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            if (attackDirection.x > 0)
                animator.SetTrigger("MeleeRight");
            else
                animator.SetTrigger("MeleeLeft");
        }
        else
        {
            if (attackDirection.y > 0)
                animator.SetTrigger("MeleeBack");
            else
                animator.SetTrigger("MeleeFront");
        }

        StartCoroutine(DelayedMeleeHitDetection(0.2f));
    }

    private IEnumerator DelayedMeleeHitDetection(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange);
        foreach (Collider2D collider in hitObjects)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            Enemy enemyComponent = collider.GetComponent<Enemy>();

            if (enemyHealth != null && enemyHealth.currentHealth > 0 && enemyComponent.activeSymbols.Count == 0)
            {
                enemyHealth.TakeDamage(meleeDamage);
            }
        }

        StartCoroutine(ResetMeleeAnimation());
    }

    private IEnumerator ResetMeleeAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        animator.ResetTrigger("MeleeRight");
        animator.ResetTrigger("MeleeLeft");
        animator.ResetTrigger("MeleeBack");
        animator.ResetTrigger("MeleeFront");

        player.GetComponent<PlayerMovement>().isAttacking = false;
    }

    private void OnDestroy()
    {
        inputManager.OnShoot -= Shoot;
    }
}