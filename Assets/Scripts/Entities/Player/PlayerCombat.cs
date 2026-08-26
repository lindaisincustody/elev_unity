using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerCombat : Component
{
    [SerializeField] float meleeAttackRange = 2f;
    [SerializeField] int meleeDamage = 5;
    [SerializeField] float meleeCooldown = 0.7f;

    private float nextMeleeTime = 0f;
    private Player player;
    private Animator animator;
    private InputManager inputManager;

    private static readonly int MeleeRightHash = Animator.StringToHash("MeleeRight");
    private static readonly int MeleeLeftHash = Animator.StringToHash("MeleeLeft");
    private static readonly int MeleeBackHash = Animator.StringToHash("MeleeBack");
    private static readonly int MeleeFrontHash = Animator.StringToHash("MeleeFront");

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        inputManager = InputManager.Instance;
        inputManager.OnShoot += Shoot;
    }

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

    private void Shoot()
    {
        if (!SanityManager.Instance.IsPlayerInUnderworld)
            return;

        PerformMeleeAttack();
    }

    private void PerformMeleeAttack()
    {
        if (Time.time < nextMeleeTime)
            return;

        nextMeleeTime = Time.time + meleeCooldown;

        player.GetComponent<PlayerMovement>().isAttacking = true;

        Vector3 mw = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mw.z = 0;
        Vector2 d = (mw - transform.position).normalized;

        int state = Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? MeleeRightHash : MeleeLeftHash)
            : (d.y > 0 ? MeleeBackHash : MeleeFrontHash);

        animator.SetTrigger(state);

        StartCoroutine(DelayedMeleeHitDetection(0.2f));
    }

    private IEnumerator DelayedMeleeHitDetection(float delay)
    {
        yield return new WaitForSeconds(delay);
        var hits = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange);
        foreach (var c in hits)
        {
            var h = c.GetComponent<EnemyHealth>();
            var e = c.GetComponent<Enemy>();
            if (h != null && h.currentHealth > 0 && e.activeSymbols.Count == 0)
                h.TakeDamage(meleeDamage);
        }
        StartCoroutine(ResetMeleeAnimation());
    }

    private IEnumerator ResetMeleeAnimation()
    {
        yield return new WaitForSeconds(0.05f);
        animator.ResetTrigger(MeleeRightHash);
        animator.ResetTrigger(MeleeLeftHash);
        animator.ResetTrigger(MeleeBackHash);
        animator.ResetTrigger(MeleeFrontHash);
        player.GetComponent<PlayerMovement>().isAttacking = false;
    }

    private void OnDestroy()
    {
        inputManager.OnShoot -= Shoot;
    }
}
