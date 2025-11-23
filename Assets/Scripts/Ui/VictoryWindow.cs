using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryWindow : UiWindowObject
{
    [SerializeField] private Button backButton;
    [SerializeField] private Text victoryText;

    [Multiline]
    [SerializeField]
    private string victoryTextFormat;

    private new void Awake()
    {
        base.Awake();
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    public void Setup(int secondsToBeat)
    {
        victoryText.text = string.Format(victoryTextFormat, secondsToBeat);
    }

    private void OnBackButtonClick()
    {
        SceneManager.LoadScene(Constants.MainMenuSceneName);
    }
}