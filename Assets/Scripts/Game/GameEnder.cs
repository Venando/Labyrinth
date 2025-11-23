using System;
using UnityEngine;

public class GameEnder : MonoBehaviour, ISavable
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameUiManager gameUiManager;
    [SerializeField] private GameTimeManager gameTimeManager;

    private Vector3Int endCoordinates;

    public string SaveKey => gameObject.name;

    private void Awake()
    {
        characterController.StepOnTile += OnStepOnTile;
    }

    public void Setup(Vector3Int endGameCoordinates)
    {
        endCoordinates = endGameCoordinates;
    }

    private void OnStepOnTile(Vector3Int coordinates)
    {
        if (endCoordinates == coordinates)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        characterController.StopCharacter();

        var victoryWindow = gameUiManager.Open<VictoryWindow>();

        victoryWindow.Setup(gameTimeManager.GetTimeSinceGameStartInSeconds());
    }

    private void OnDestroy()
    {
        if (characterController != null)
        {
            characterController.StepOnTile -= OnStepOnTile;
        }
    }

    public string CaptureState()
    {
        var coordinatesWrapper = new CoordinatesWrapper { endCoordinates = endCoordinates };
        return JsonUtility.ToJson(coordinatesWrapper);
    }

    public void RestoreState(string json)
    {
        var coordinatesWrapper = JsonUtility.FromJson<CoordinatesWrapper>(json);
        endCoordinates = coordinatesWrapper.endCoordinates;
    }

    [Serializable]
    public class CoordinatesWrapper
    {
        public Vector3Int endCoordinates;
    }
}
