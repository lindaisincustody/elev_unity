using System.Collections;
using UnityEngine;

public class DissolveEffect
{
    private readonly Material material;
    private readonly float dissolveTime;

    private readonly int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private readonly int _verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    public DissolveEffect(Material mat, float time)
    {
        material = mat;
        dissolveTime = time;
    }

    public void Vanish()
    {
        CoroutineRunner.RunCoroutine(VanishCoroutine());
    }

    public void Appear()
    {
        CoroutineRunner.RunCoroutine(AppearCoroutine());
    }

    private IEnumerator VanishCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float value = Mathf.Lerp(0, 1.1f, elapsedTime / dissolveTime);
            material.SetFloat(_dissolveAmount, value);
            material.SetFloat(_verticalDissolveAmount, value);
            yield return null;
        }
    }

    private IEnumerator AppearCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float value = Mathf.Lerp(1.1f, 0, elapsedTime / dissolveTime);
            material.SetFloat(_dissolveAmount, value);
            material.SetFloat(_verticalDissolveAmount, value);
            yield return null;
        }
    }
}
