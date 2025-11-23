using UnityEngine;

[CreateAssetMenu]
public class DefaultOptionsScriptableObject : ScriptableObject
{
    [SerializeField] private int characterMoveSpeed = 5;
    [SerializeField] private int mazeSize = 12;

    public int CharacterMoveSpeed => characterMoveSpeed;
    public int MazeSize => mazeSize;
}