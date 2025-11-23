using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : UiWindowObject
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private Button pauseButton;

    private new void Awake()
    {
        base.Awake();
        pauseButton.onClick.AddListener(OnPauseButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseButton();
        }
    } 

    private void OnPauseButton()
    {
        UiManager.Open<PauseMenu>();
    }

    public void StartTimer(double startTime)
    {
        gameTimer.StartTimer(startTime);
    }
}
