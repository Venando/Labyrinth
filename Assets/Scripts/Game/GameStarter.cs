using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private MazeGenerator mazeGenerator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameUiManager gameUiManager;
    [SerializeField] private GameTimeManager gameTimeManager;
    [SerializeField] private GameEnder gameEnder;

    private void Start()
    {
        SetOptions();

        if (GameSceneSettings.IsSaveLoading)
        {
            LoadGameState();
        }
        else
        {
            StartNewGame();
        }

        StartGameTimer();
    }

    private void SetOptions()
    {
        var options = OptionsManager.Instance;

        mazeGenerator.SetMazeSize(options.MazeSize, options.MazeSize);
        characterController.SetMoveSpeed(options.CharacterMovementSpeed);
    }

    private void LoadGameState()
    {
        SaveManager.LoadAll();
    }
    
    private void StartNewGame()
    {
        mazeGenerator.GenerateAllTiles();
        characterController.transform.position = mazeGenerator.GetCenterPosition();
        gameTimeManager.StartTime();
        gameEnder.Setup(mazeGenerator.GetExitCoordinates());
    }

    private void StartGameTimer()
    {
        GameHud gameHud = gameUiManager.Get<GameHud>();
        gameHud.StartTimer(gameTimeManager.GetGameStartTime());
    }
}
