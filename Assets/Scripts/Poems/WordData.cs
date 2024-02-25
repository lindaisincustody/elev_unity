using UnityEngine;

[System.Serializable]
public class Word
{
    public string word;
    public float wordLength;
    [Range(0f, 1f)] public float strengthWeight;
    [Range(0f, 1f)] public float intelligenceWeight;
    [Range(0f, 1f)] public float coordinationWeight;
    [Range(0f, 1f)] public float neutralWeight;
}

[CreateAssetMenu(fileName = "WordData", menuName = "Custom/Word Data")]
public class WordData : ScriptableObject
{
    public string Poem;
    public Vector2 WordPosition;
    public Vector2 oldWordPosition;
    public Word[] words = new Word[9];
}
