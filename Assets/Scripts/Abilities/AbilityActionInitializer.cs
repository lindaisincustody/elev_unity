using System;
using UnityEngine;

public class AbilityActionsInitializer : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private LetterDrawing letterDrawing;

    private void Start()
    {
        AbilityActionRegistry.Instance.RegisterAction("_diagup", () =>
        {
            var playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
            var diagup = playerAbilities?.Abilities.Find(a => a is DiagupAbility) as DiagupAbility;

            if (diagup != null)
            {
                diagup.Activate();
                Vector2[] pts = letterDrawing.GetDrawnPoints();
                if (pts != null && pts.Length > 0)
                {
                    diagup.SpawnSlashAt(pts[pts.Length - 1]);
                }
            }
            else Debug.LogWarning("Player does not have the Diagup ability.");
        });
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

        System.Action heartActiob = () =>
        {
            PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
            HealAbility heakAbility = playerAbilities?.Abilities.Find(a => a is HealAbility) as HealAbility;
            if (heakAbility != null)
            {
                heakAbility.Activate();
            }
            else
            {
                Debug.Log("Player does not have the Arrow ability.");
            }
        };

        System.Action leoAction = () =>
        {
            PlayerAbilities playerAbilities = Player.instance.GetComponent<PlayerAbilities>();
            SplashAbility splashAbility = playerAbilities?.Abilities
                .Find(a => a is SplashAbility) as SplashAbility;

            if (splashAbility != null)
            {
                splashAbility.Activate();
                // pass any MonoBehaviour to run the coroutine; letterDrawing works fine
                splashAbility.SpawnSplashEffect(letterDrawing, Player.instance.transform);
            }
            else
            {
                Debug.Log("Player does not have the Splash ability.");
            }
        };

        AbilityActionRegistry.Instance.RegisterAction("_Rightarrow", arrowAction);
        AbilityActionRegistry.Instance.RegisterAction("_downarrow", arrowAction);
        AbilityActionRegistry.Instance.RegisterAction("_Heart", heartActiob);
        AbilityActionRegistry.Instance.RegisterAction("_Leo", leoAction);
    }
}