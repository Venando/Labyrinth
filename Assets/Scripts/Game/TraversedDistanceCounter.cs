using System;
using UnityEngine;

public class TraversedDistanceCounter : MonoBehaviour, ISavable
{
    [SerializeField] private CharacterController characterController;

    private int counter;

    public event Action<int> CounterValueChanged;

    public string SaveKey => gameObject.name;

    private void Awake()
    {
        characterController.StepOnTile += OnStepOnTile;
    }

    private void OnStepOnTile(Vector3Int coordinates)
    {
        SetCounter(counter + 1);
    }

    private void SetCounter(int value)
    {
        counter = value;
        CounterValueChanged?.Invoke(counter);
    }

    public string CaptureState()
    {
        return counter.ToString();
    }

    public void RestoreState(string json)
    {
        int.TryParse(json, out counter);
        SetCounter(counter);
    }
}
