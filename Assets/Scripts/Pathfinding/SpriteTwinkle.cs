using UnityEngine;

public class SpriteTwinkle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float period = 1.6f;

    private void Update()
    {
        float pulse = 0.5f - 0.5f * Mathf.Cos(Time.time / period * Mathf.PI * 2f);
        target.sprite = frames[Mathf.Min((int)(pulse * frames.Length), frames.Length - 1)];
    }
}
