using UnityEngine;
using UnityEngine.UI;

public class DistancePanel : MonoBehaviour
{
    [SerializeField] private TraversedDistanceCounter movementCounter;
    [SerializeField] private Text text;
    [SerializeField] private string textFormat;

    private void Awake()
    {
        movementCounter.CounterValueChanged += OnDistanceChanged;
        OnDistanceChanged(0);
    }

    private void OnDistanceChanged(int distance)
    {
        text.text = string.Format(textFormat, distance);
    }

    private void OnDestroy()
    {
        if (movementCounter != null)
        {
            movementCounter.CounterValueChanged -= OnDistanceChanged;
        }
    }
}