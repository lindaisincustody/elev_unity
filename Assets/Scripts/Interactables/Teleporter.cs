using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Teleporter : Interactable
{
    [SerializeField] private UnityEvent OnTeleport;
    [SerializeField] public bool Unlocked = true;
    [SerializeField] public Teleport teleport;

    [Header("Scene To Load")] [SerializeField]
    Scene sceneName;

    [Header("Position To Move")] [SerializeField]
    float scene_X;

    [SerializeField] float scene_Y;
    [SerializeField] Transform position;

    private SceneController sceneController;

    protected override void Start()
    {
        base.Start();

        sceneController = UIManager.Instance.Get<SceneController>();

        if (!Unlocked)
            gameObject.SetActive(false);

        SanityManager.Instance.OnWorldChange += ChangeTeleportState;
    }

    private void ChangeTeleportState()
    {
        if (SanityManager.Instance.IsPlayerInUnderworld)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (Unlocked)
                gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player == null)
                player = Player.instance;
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
            {
                if (position != null)
                    StartCoroutine(sceneController.LoadInScene(position.position.x, position.position.y));
                else
                    StartCoroutine(sceneController.LoadInScene(scene_X, scene_Y));
                OnTeleport?.Invoke();
            }
            else if (teleport == Teleport.NewScene)
                StartCoroutine(sceneController.LoadScene(GetSceneName()));
        }
    }

    private string GetSceneName()
    {
        switch (sceneName)
        {
            case Scene.Main:
                return Constants.SceneNames.MainScene;
            case Scene.Station:
                return Constants.SceneNames.TrainStation;
            case Scene.Hotel:
                return Constants.SceneNames.HotelScene;
            default:
                return ""; // Default return, you can handle this case as needed.
        }
    }

    private void OnDestroy()
    {
        SanityManager.Instance.OnWorldChange -= ChangeTeleportState;
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