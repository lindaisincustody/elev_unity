using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class PlayerController
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource moveSound;
    private InputManager playerInput;
    private BattlePlayerController battlePlayerController;

    [SetUp]
    public void Setup()
    {
        GameObject gameObject = new GameObject();
        rb = gameObject.AddComponent<Rigidbody2D>();
        animator = gameObject.AddComponent<Animator>();
        moveSound = gameObject.AddComponent<AudioSource>();
        playerInput = gameObject.AddComponent<InputManager>();
        battlePlayerController = gameObject.AddComponent<BattlePlayerController>();

        playerMovement = gameObject.AddComponent<PlayerMovement>();
        playerMovement.rb = rb;
        playerMovement.animator = animator;
        playerMovement.moveSound = moveSound;
        playerMovement.playerInput = playerInput;
        playerMovement.battlePlayerController = battlePlayerController;
    }

    [Test]
    public void PlayerMovement_Initialization()
    {
        Assert.IsNotNull(playerMovement.rb);
        Assert.IsNotNull(playerMovement.animator);
        Assert.IsNotNull(playerMovement.moveSound);
        Assert.IsNotNull(playerMovement.playerInput);
        Assert.IsNotNull(playerMovement.battlePlayerController);
    }

    [Test]
    public void PlayerMovement_StartMovementSound()
    {
        playerMovement.moveSound = null;
        playerMovement.StartMovementSound(); // Should not throw error if moveSound is not assigned
    }

    [Test]
    public void PlayerMovement_StopMovementSound()
    {
        playerMovement.StopMovementSound(); // Should not throw error if moveSound is not playing
    }

    [Test]
    public void PlayerMovement_AdjustSoundProperties()
    {
        playerMovement.moveSound = null;
        playerMovement.AdjustSoundProperties(); // Should not throw error if moveSound is not assigned
    }

    [Test]
    public void PlayerMovement_SetMovement()
    {
        playerMovement.SetMovement(true);
        Assert.IsTrue(playerMovement._canMove);
    }

}
