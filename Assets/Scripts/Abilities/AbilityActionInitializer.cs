using System;
using UnityEngine;

public class AbilityActionsInitializer : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private LetterDrawing letterDrawing;

    private void Start()
    {
        AbilityActionRegistry.Instance.RegisterAction("_square", () =>
        {
            PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
            WallAbility wallAbility = playerAbilities?.Abilities.Find(a => a is WallAbility) as WallAbility;
            if (wallAbility != null)
            {
                wallAbility.Activate();
                Vector2[] points = letterDrawing.GetDrawnPoints();
                if (points != null)
                {
                    wallAbility.SpawnWall(points, letterDrawing, letterDrawing.secondaryLineRenderer,
                        letterDrawing.groundMaterial, letterDrawing.trippyTransparentMaterial);
                }
            }
            else
            {
                Debug.Log("Player does not have the Wall ability.");
            }
        });

        System.Action arrowAction = () =>
        {
            PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
            ArrowAbility arrowAbility = playerAbilities?.Abilities.Find(a => a is ArrowAbility) as ArrowAbility;
            if (arrowAbility != null)
            {
                arrowAbility.Activate();
                Enemy targetEnemy = Player.instance.Get<PlayerCombat>().GetNearestEnemy();
                if (targetEnemy != null)
                {
                    arrowAbility.SpawnArrowEffect(letterDrawing, targetEnemy.transform);
                }
            }
            else
            {
                Debug.Log("Player does not have the Arrow ability.");
            }
        };
        AbilityActionRegistry.Instance.RegisterAction("_Rightarrow", arrowAction);
        AbilityActionRegistry.Instance.RegisterAction("_downarrow", arrowAction);
    }
}