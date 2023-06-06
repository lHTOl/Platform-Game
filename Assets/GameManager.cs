using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ControlUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject DiedUI;
    [SerializeField] private GameObject FinishUI;
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject JumpButton;
    AdsGameInSystem adsGameInSystem;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        adsGameInSystem = GameObject.FindGameObjectWithTag("AdsSystem").GetComponent<AdsGameInSystem>();
    }

    public void Pause()
    {
        if (Time.timeScale == 1 && ControlUI.activeSelf && !PauseUI.activeSelf)
        {
            Time.timeScale = 0;
            ControlUI.SetActive(false);
            PauseButton.SetActive(false);
            JumpButton.SetActive(false);
            PauseUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            ControlUI.SetActive(true);
            PauseButton.SetActive(true);
            JumpButton.SetActive(true);
            PauseUI.SetActive(false);
        }
    }

    public void Died()
    {
        ControlUI.SetActive(false);
        PauseButton.SetActive(false);
        JumpButton.SetActive(false);
        DiedUI.SetActive(true);
        if(!adsGameInSystem.interstitialAdShow)
        {
            adsGameInSystem.ShowInterstitialAd();
        }
    }

    public void Finish()
    {
        Time.timeScale = 0;
        ControlUI.SetActive(false);
        PauseButton.SetActive(false);
        JumpButton.SetActive(false);
        FinishUI.SetActive(true);
        if (!adsGameInSystem.interstitialAdShow)
        {
            adsGameInSystem.ShowInterstitialAd();
        }
    }
}
