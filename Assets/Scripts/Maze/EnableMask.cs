using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableMask : MonoBehaviour
{
    public float maskActivationTime;
    public float maskDeactivationTime;
    public float minMaskSize = 10f;
    public float maxMaskSize = 100f;
    [SerializeField] private Transform maskSprite;
    public List<SpriteRenderer> mazeWalls = new List<SpriteRenderer>();
    private bool flipper = true;
    public AnimationCurve scaleCurve;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            UseMaskEffect();
    }

    public void AddMazeCell(MazeCell cell)
    {
        mazeWalls.AddRange(cell.GetComponentsInChildren<SpriteRenderer>());
    }

    private IEnumerator ActivateInvisibleWallEffect()
    {
        float elapsedTime = 0f;
        maskSprite.localScale = new Vector2(maxMaskSize, maxMaskSize);

        while (elapsedTime < maskActivationTime)
        {
            elapsedTime += Time.deltaTime;
            float curveValue = scaleCurve.Evaluate(elapsedTime / maskActivationTime);
            float newScale = Mathf.Lerp(maxMaskSize, minMaskSize, curveValue);
            maskSprite.localScale = new Vector2(newScale, newScale);
            yield return null;
        }
    }

    private IEnumerator DeactivateInvisibleWallEffect()
    {
        float elapsedTime = 0f;
        maskSprite.localScale = new Vector2(minMaskSize, minMaskSize);

        while (elapsedTime < maskDeactivationTime)
        {
            elapsedTime += Time.deltaTime;
            float curveValue = scaleCurve.Evaluate(elapsedTime / maskDeactivationTime);
            float newScale = Mathf.Lerp(minMaskSize, maxMaskSize, curveValue);
            maskSprite.localScale = new Vector2(newScale, newScale);
            yield return null;
        }
        DeactivateMaskEffect();
    }


    private void UseMaskEffect()
    {
        if (flipper)
        {
            ActivateMaskEffect();
            StartCoroutine(ActivateInvisibleWallEffect());
        }
        else
        {
            StartCoroutine(DeactivateInvisibleWallEffect());
        }
        flipper = !flipper;
    }

    private void ActivateMaskEffect()
    {
        foreach (var item in mazeWalls)
        {
            item.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    private void DeactivateMaskEffect()
    {
        foreach (var item in mazeWalls)
        {
            item.maskInteraction = SpriteMaskInteraction.None;
        }
    }
}
