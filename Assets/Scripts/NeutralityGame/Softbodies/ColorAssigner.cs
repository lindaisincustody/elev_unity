using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAssigner : MonoBehaviour
{
    public ParticleSystem ps;
    public float psLifetime = 3f;
    private SpriteRenderer sprite;
    private JelloPSContorller psController;
    private bool isOverwridden = false;
    Coroutine coroutine;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        psController = new JelloPSContorller(); // Create an instance of JelloPSContorller
    }

    private void Start()
    {
        AssignRandomBrightColor();
    }

    private void AssignRandomBrightColor()
    {
        if (isOverwridden)
            return;
        // Generate random values for RGB components
        float r = Random.Range(0.2f, 1f);
        float g = Random.Range(0.2f, 1f);
        float b = Random.Range(0.2f, 1f);

        // Assign the bright color to the SpriteRenderer
        sprite.color = new Color(r, g, b);
        psController.ChangeParticleColor(ps, sprite.color);

        coroutine = StartCoroutine(DisablePS());
    }

    public void AssignBrightColor(Color newcolor)
    {
        isOverwridden = true;
        if (coroutine != null)
            StopCoroutine(coroutine);
        sprite.color = newcolor;
        psController = new JelloPSContorller();
        psController.ChangeParticleColor(ps, newcolor);

        StartCoroutine(DisablePS());
    }

    private IEnumerator DisablePS()
    {
        float elapsedTime = 0f;
        float startEmissionRate = ps.emissionRate;
        float targetEmissionRate = 0f;

        while (elapsedTime < psLifetime)
        {
            float newEmissionRate = Mathf.Lerp(startEmissionRate, targetEmissionRate, elapsedTime / psLifetime);
            var emission = ps.emission;
            emission.rateOverTime = newEmissionRate;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ps.gameObject.SetActive(false);
    }
}
