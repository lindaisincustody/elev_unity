using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WritingEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wordHolder;
    [SerializeField] private RectTransform highlightImage;
    [SerializeField] private RectTransform text;
    [SerializeField] private float speed = 2.0f;
    private RectTransform effect;
    private float EffectInitialPosition;

    private float textOffscreenPositionX = 184.5f;

    private void Start()
    {
        effect = GetComponent<RectTransform>();
        EffectInitialPosition = effect.anchoredPosition.x;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
            StartEffect();
        if (Input.GetKeyDown(KeyCode.R))
            ResetEffect();*/
    }

    public void StartEffect(string word, WordFiller caller)
    {
        wordHolder.text = word;
        effect.anchoredPosition -= new Vector2(textOffscreenPositionX, 0);
        text.anchoredPosition = new Vector3(textOffscreenPositionX, 0, 0);
        StartCoroutine(MoveElementsCoroutine(caller));
    }
    private IEnumerator ResetEffect()
    {
        yield return new WaitForSeconds(3f);
        text.anchoredPosition = new Vector2(0, text.anchoredPosition.y);
        highlightImage.anchoredPosition = new Vector2(0, highlightImage.anchoredPosition.y);
        effect.anchoredPosition = new Vector2(EffectInitialPosition, effect.anchoredPosition.y);
    }

    private IEnumerator MoveElementsCoroutine(WordFiller caller)
    {
        while (true)
        {
            if (text.anchoredPosition.x < 0.001f)
            {
                StartCoroutine(ResetEffect());
                caller.StartClosingBook();
                yield break;
            }
            highlightImage.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);
            text.anchoredPosition += new Vector2(-speed * Time.deltaTime, 0);
            effect.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
