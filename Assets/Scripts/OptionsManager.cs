using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    private const string CharacterMoveSpeedKey = nameof(CharacterMoveSpeedKey);
    private const string MazeSizeKey = nameof(MazeSizeKey);


    public int CharacterMovementSpeed
    {
        get => GetIntValue(CharacterMoveSpeedKey, defaultOptions.CharacterMoveSpeed);
        set => SetIntValue(CharacterMoveSpeedKey, value);
    }

    public int MazeSize
    {
        get => GetIntValue(MazeSizeKey, defaultOptions.MazeSize);
        set => SetIntValue(MazeSizeKey, value);
    }

    [SerializeField] private DefaultOptionsScriptableObject defaultOptions;

    private static OptionsManager instance;
    public static OptionsManager Instance => instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this);
    } 

    public void ResetOptions()
    {
        PlayerPrefs.DeleteKey(CharacterMoveSpeedKey);
        PlayerPrefs.DeleteKey(MazeSizeKey);
    }

    private int GetIntValue(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    private void SetIntValue(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

}