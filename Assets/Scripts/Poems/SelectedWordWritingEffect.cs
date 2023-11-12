using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectedWordWritingEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wordHolder;
    [SerializeField] private RectTransform highlightImage;
    [SerializeField] private RectTransform text;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private ParticleMask particleMask;
    private RectTransform effect;
    private float EffectInitialPosition;

    private void Start()
    {
        effect = GetComponent<RectTransform>();
        EffectInitialPosition = effect.anchoredPosition.x;
    }

    private void OnEnable()
    {
        particleMask.calculateDuration(wordHolder.text);
    }

    public void SelectedWordEffect()
    {
        highlightImage.gameObject.SetActive(true);
        StartCoroutine(MoveSelectedElementsCoroutine());
    }

    private IEnumerator ResetEffect()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log(text.anchoredPosition.y);
        float originalLeft = 0f;
        float originalRight = 40f;
        float originalTop = 20f;
        float originalBottom = -20f;

        // Set the offset values directly
        text.offsetMin = new Vector2(originalLeft, originalBottom);
        text.offsetMax = new Vector2(originalRight, originalTop);
        highlightImage.anchoredPosition = new Vector2(0, highlightImage.anchoredPosition.y);
        effect.anchoredPosition = new Vector2(EffectInitialPosition, effect.anchoredPosition.y);
        highlightImage.gameObject.SetActive(false);
    }

    private IEnumerator MoveSelectedElementsCoroutine()
    {
        while (true)
        {
            if (text.anchoredPosition.x < -200)
            {
                StartCoroutine(ResetEffect());
                yield break;
            }
            highlightImage.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);
            text.anchoredPosition += new Vector2(-speed * Time.deltaTime, 0);
            effect.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
