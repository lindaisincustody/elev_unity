using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataDeleteManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SavingWrapper.Instance.DeleteAllData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
