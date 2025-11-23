using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : UiWindowObject
{
    [SerializeField] private OptionsButton characterSpeedOptionButton;
    [SerializeField] private OptionsButton mazeSizeOptionButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    private new void Awake()
    {
        base.Awake();
        characterSpeedOptionButton.onValueChanged.AddListener(OnCharacterSpeedOptionChanged);
        mazeSizeOptionButton.onValueChanged.AddListener(OnMazeSizeOptionChanged);
        resetButton.onClick.AddListener(OnResetButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnEnable()
    {
        UpdateValues();
    }

    private void UpdateValues()
    {
        var optionManager = OptionsManager.Instance;
        characterSpeedOptionButton.Value = optionManager.CharacterMovementSpeed;
        mazeSizeOptionButton.Value = optionManager.MazeSize;
    }

    private void OnCharacterSpeedOptionChanged(int value)
    {
        OptionsManager.Instance.CharacterMovementSpeed = value;
    }
    
    private void OnMazeSizeOptionChanged(int value)
    {
        OptionsManager.Instance.MazeSize = value;
    }

    private void OnResetButtonClick()
    {
        OptionsManager.Instance.ResetOptions();
        UpdateValues();
    }

    private void OnBackButtonClick()
    {
        UiManager.CloseLast();
    }
}