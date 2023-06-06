using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class UISystem : MonoBehaviour
{
    public void ChangeScene(string LoadScene)
    {
        if (LoadScene != "")
            SceneManager.LoadScene(LoadScene);
        else
            Debug.Log("Çok Yakýnda Hizmetinizde.");
    }
    public void TimeScale()
    {
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Finish()
    {
        string LevelName = SceneManager.GetActiveScene().name;
        int Level = Convert.ToInt32(LevelName.Substring(5, 1));
        Level++;
        string NextLevel = "Level" + Level;
        SceneManager.LoadScene(NextLevel);
    }
}
