using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleScoreVisualizer : MonoBehaviour
{
    [SerializeField] private NeutralityManager manager;
    [SerializeField] Image scoreColor;
    Slider visual;
    Color normalScoreColor = new Color(0, 1, 0.58f);

    int maxScore = 0;
    int differnceLoss = 0;

    private void Awake()
    {
        visual = GetComponent<Slider>();
        manager.gameStart += SetUpValues;
    }

    private void SetUpValues()
    {
        maxScore = manager.GetScoreToWin();
        differnceLoss = manager.GetDifferenceToLose();
    }

    public void UpdateVisualizer(int newScore, int otherScaleScore)
    {
        float percent = (float)newScore / maxScore;
        visual.value = percent;

        if (newScore - otherScaleScore > differnceLoss)
            scoreColor.color = Color.yellow;
        else
            scoreColor.color = normalScoreColor;

        if (newScore - otherScaleScore > differnceLoss + 1)
        {
            scoreColor.color = Color.red;
            manager.Lose();
        }

        if (newScore >= maxScore)
            manager.Win();
    }

    private void OnDestroy()
    {
        manager.gameStart -= SetUpValues;
    }
}
