using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : UiWindowObject
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button leaveButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        saveButton.onClick.AddListener(SaveGame);
        leaveButton.onClick.AddListener(LeaveGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    } 

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void ResumeGame()
    {
        UiManager.CloseLast();
    }
    
    private void SaveGame()
    {
        SaveManager.SaveAll();
    }

    private void LeaveGame()
    {
        SceneManager.LoadScene(Constants.MainMenuSceneName);
    }
}
