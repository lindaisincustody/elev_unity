using UnityEngine;
using NUnit.Framework;
using TMPro;
using UnityEngine.UI;

public class DialogueControllerTests
{
    private GameObject gameObject;
    private DialogueController dialogueController;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        dialogueController = gameObject.AddComponent<DialogueController>();
        GameObject cursorObject = new GameObject("Cursor");
        // Manually initializing components and mocks as in the Start() method of DialogueController
        dialogueController.CanvasBG = new GameObject();
        dialogueController.DialogueObj = new GameObject();
        dialogueController.MinigamesBoxObj = new GameObject();
        dialogueController.dialogueText = new GameObject().AddComponent<TextMeshProUGUI>();
        dialogueController.nameText = new GameObject().AddComponent<TextMeshProUGUI>();
        dialogueController.mainCharacterImage = new GameObject().AddComponent<Image>();
        dialogueController.otherCharacterImage = new GameObject().AddComponent<Image>();
        dialogueController.mainCharacterImageHolder = new GameObject();
        dialogueController.otherCharacterImageHolder = new GameObject();
        dialogueController.particles = new GameObject().AddComponent<AttributeParticles>();
        dialogueController.cursor = new GameObject().AddComponent<CursorController>();
        dialogueController.minigamebox = new GameObject().AddComponent<UIElementsHolder>();
        dialogueController.minigameUI = new MinigameUI(dialogueController.MinigamesBoxObj, new GameObject(), new GameObject().AddComponent<Image>(), new GameObject().AddComponent<Image>(), new GameObject().AddComponent<Image>(), new GameObject().AddComponent<Image>());
        dialogueController.dialogueUI = new DialogueUI(dialogueController.DialogueObj, dialogueController.dialogueText, dialogueController.nameText, dialogueController.mainCharacterImage, dialogueController.otherCharacterImage, dialogueController.mainCharacterImageHolder, dialogueController.otherCharacterImageHolder, dialogueController.minigameUI);
        dialogueController.cursor = cursorObject.AddComponent<CursorController>();
        dialogueController.cursor.gameObject.SetActive(false);  // Initially deactivated, simulate real game conditions
        // Setup InventoryUI instance if it is a singleton used within ActivateDialogue
        var inventoryUI = new GameObject().AddComponent<InventoryUI>();
        InventoryUI.Instance = inventoryUI;  // Assume InventoryUI has a public static property for the instance
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(gameObject);
    }




    [Test]
    public void ActivateDialogue_SetsIsDialogueActiveTrue()
    {
        DialogueData mockData = ScriptableObject.CreateInstance<DialogueData>();
        // Assume these methods exist to correctly set up mock data
        mockData.mainCharacterImage = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        mockData.otherCharacterImage = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        DialogueTrigger mockTrigger = new GameObject().AddComponent<DialogueTrigger>();

        dialogueController.ActivateDialogue(mockData, mockTrigger);

        Assert.IsTrue(dialogueController.isDialogueActive);
    }

    [Test]
    public void ExitDialogue_SetsIsDialogueActiveFalse()
    {
        // Setup - assume dialogue is initially active
        dialogueController.ActivateDialogue(ScriptableObject.CreateInstance<DialogueData>(), new GameObject().AddComponent<DialogueTrigger>());
        Assert.IsTrue(dialogueController.isDialogueActive); // Ensure it's active before exit

        // Act
        dialogueController.ExitDialogue();

        // Assert
        Assert.IsFalse(dialogueController.isDialogueActive);
        Assert.IsNull(dialogueController.dialogueData);
        Assert.IsFalse(dialogueController.minigameUI.isActive); // Assuming IsVisible property exists to check UI visibility
    }

    [Test]
    public void ShowNextDialogueLine_AdvancesDialogueLineAndStartsCoroutine()
    {
        // Setup
        DialogueData mockData = ScriptableObject.CreateInstance<DialogueData>();
        mockData.textList = new DialogueData.CharacterData[]
        {
        new DialogueData.CharacterData { dialogueLineText = "Hello", isYourText = true },
        new DialogueData.CharacterData { dialogueLineText = "World", isYourText = false }
        };
        dialogueController.ActivateDialogue(mockData, new GameObject().AddComponent<DialogueTrigger>());

        // Act
        dialogueController.ShowNextDialogueLine();
        var initialLine = dialogueController.currentDialogueLine;

        // Wait for the coroutine to potentially start
        dialogueController.StartCoroutine(dialogueController.ShowText()); // Simulate coroutine continuation

        // Assert
        Assert.AreEqual(1, initialLine); // Check if line has advanced
        Assert.IsNotNull(dialogueController.dialogueCoroutine); // Check if coroutine is set
    }

    [Test]
    public void NextAction_CompletesTextDisplayIfNotFullyShown()
    {
        // Setup
        DialogueData mockData = ScriptableObject.CreateInstance<DialogueData>();
        mockData.textList = new DialogueData.CharacterData[]
        {
        new DialogueData.CharacterData { dialogueLineText = "This is a test", isYourText = true }
        };
        dialogueController.ActivateDialogue(mockData, new GameObject().AddComponent<DialogueTrigger>());
        dialogueController.ShowNextDialogueLine();
        dialogueController.NextAction(); // Partial display

        // Act
        dialogueController.NextAction(); // Should complete display

        // Assert
        Assert.AreEqual("This is a test", dialogueController.dialogueText.text);
    }

    [Test]
    public void ShowMinigameOptions_ActivatesMinigame()
    {
        // Create and setup DialogueData with at least one dialogue line
        DialogueData mockData = ScriptableObject.CreateInstance<DialogueData>();
        mockData.textList = new DialogueData.CharacterData[]
        {
        new DialogueData.CharacterData { dialogueLineText = "Test dialogue line", isYourText = true }
        };
        mockData.activateFight = true; // Ensure this is true to activate minigame options after dialogue

        // Setup dialogue controller to use the mock data
        dialogueController.ActivateDialogue(mockData, new GameObject().AddComponent<DialogueTrigger>());

        // Set currentDialogueLine to the end of the textList to simulate reaching the end of the dialogue
        dialogueController.currentDialogueLine = mockData.textList.Length; // Now valid because textList is initialized

        // Act: Attempt to show minigame options
        dialogueController.ShowMinigameOptions();

        // Assert: Check if the minigame UI is active and the dialogue UI is inactive
        Assert.IsFalse(dialogueController.isDialogueActive);
        Assert.IsTrue(dialogueController.isMinigamesBoxActive);
    }



}
