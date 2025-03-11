using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Data", menuName = "Elevator/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Sprite npcSprite;
    [TextArea] public string greetingText;
    [TextArea] public string thankYouText;
    public int requestedFloor; // The floor this NPC wants to go to.
}