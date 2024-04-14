using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Teleporter : Interactable
{
    [SerializeField] public Teleport teleport;
    [Header("Scene To Load")]
    [SerializeField] Scene sceneName;
    [Header("Position To Move")]
    [SerializeField] float scene_X;
    [SerializeField] float scene_Y;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            player.ShowInteractUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            player.ShowInteractUI(false);
        }
    }

    protected override void HandleInteract()
    {
        if (playerIsInTrigger)
        {
            base.HandleInteract();
            if (teleport == Teleport.SameScene)
                StartCoroutine(SceneController.instance.LoadInScene(scene_X, scene_Y));
            else if (teleport == Teleport.NewScene)
                StartCoroutine(SceneController.instance.LoadScene(GetSceneName()));
        }
    }

    private string GetSceneName()
    {
        switch (sceneName)
        {
            case Scene.IntelligenceGame:
                return Constants.SceneNames.IntelligenceGameScene;
            case Scene.StrengthGame:
                return Constants.SceneNames.StrengthGameScene;
            case Scene.NeutralityGame:
                return Constants.SceneNames.NeutralityGameScene;
            case Scene.CoordinationGame:
                return Constants.SceneNames.CoordinationGameScene;
            case Scene.Main:
                return Constants.SceneNames.MainScene;
            case Scene.Station:
                return Constants.SceneNames.StationScene;
            case Scene.Hotel:
                return Constants.SceneNames.HotelScene;
            default:
                return ""; // Default return, you can handle this case as needed.
        }
    }
}

public enum Teleport
{
    SameScene,
    NewScene
}

public enum Scene
{
    IntelligenceGame,
    StrengthGame,
    NeutralityGame,
    CoordinationGame,
    Main,
    Station,
    Hotel
}



