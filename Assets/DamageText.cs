using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [HideInInspector] public TextMeshPro textMesh;
    [HideInInspector] public CanvasGroup canvasGroup;

    // Called by the spawner to set up the text
    public void Initialize(int damage)
    {
        textMesh.text = damage.ToString();
        // Pop animation: scale from zero → 1
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);

        // Fly up + fade out
        float riseDistance = 1f;
        float duration = 1f;
        transform.DOMoveY(transform.position.y + riseDistance, duration).SetEase(Ease.OutCubic);
        canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject));
    }

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
