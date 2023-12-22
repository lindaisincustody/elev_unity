using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssController : MonoBehaviour
{
    public GameObject stageOneObj;
    public GameObject stageTwoObj;
    public GameObject stageThreeObj;

    private int ActiveStageIndex = 0;

    public void NextStage()
    {
        ActiveStageIndex++;
        switch (ActiveStageIndex)
        {
            case 1:
                stageOneObj.SetActive(true);
                break;
            case 2:
                stageTwoObj.SetActive(true);
                break;
            case 3:
                stageThreeObj.SetActive(true);
                break;
            case 4:
                Debug.Log("Over!");
                break;
        }
    }
}
