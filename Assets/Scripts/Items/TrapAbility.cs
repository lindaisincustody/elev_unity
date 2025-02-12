using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trap Ability", menuName = "Custom/Ability/TrapAbility")]
public class TrapAbility : Ability
{
    [SerializeField] private GameObject trapPrefab;

    private GameObject trap;

    public override void Activate()
    {
        base.Activate();
        Enemy nearestEnemy = Player.instance.PlayerCombat.GetNearestEnemy();

        if (nearestEnemy != null && trapPrefab != null)
        {
            trap = Instantiate(trapPrefab, nearestEnemy.transform.position, Quaternion.identity);
            FreezeEnemy(nearestEnemy.Get<EnemyMovement>());
        }
    }

    public void FreezeEnemy(EnemyMovement enemy)
    {
        CoroutineRunner.RunCoroutine(FreezeCoroutine(enemy));
    }

    private IEnumerator FreezeCoroutine(EnemyMovement enemy)
    {
        enemy.Freeze("TrapAbility");
        yield return new WaitForSeconds(activeTime);
        Destroy(trap);
        enemy.Unfreeze("TrapAbility");
    }
}
