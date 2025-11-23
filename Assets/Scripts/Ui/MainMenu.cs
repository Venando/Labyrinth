using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : UiWindowObject
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    private new void Awake()
    {
        base.Awake();

        newGameButton.onClick.AddListener(OnNewGameButtonClick);

        loadButton.interactable = SaveManager.HasSave();
        loadButton.onClick.AddListener(OnLoadButtonClick);

        optionsButton.onClick.AddListener(OnOptionsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnNewGameButtonClick()
    {
        GameSceneSettings.IsSaveLoading = false;
        SceneManager.LoadScene(Constants.GameSceneName);
    }

    private void OnLoadButtonClick()
    {
        GameSceneSettings.IsSaveLoading = true;
        SceneManager.LoadScene(Constants.GameSceneName);
    }

    private void OnOptionsButtonClick()
    {
        UiManager.Open<OptionsMenu>();
    }

    private void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else  
        Application.Quit();  
#endif
    }
}
