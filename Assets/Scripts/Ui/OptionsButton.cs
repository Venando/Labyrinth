using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private Text mainText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;

    [SerializeField] private string mainTextFormat = "value {0}";

    [SerializeField] private int[] defaultValues;
    [SerializeField] private int delta;
    [SerializeField] private int maxValue;
    [SerializeField] private int minValue;

    private int value;

    public UnityEvent<int> onValueChanged;

    public int Value
    {
        get
        {
            return value;
        }
        set
        {
            var newValue = Mathf.Clamp(value, minValue, maxValue);
            if (this.value == value)
                return;
            this.value = newValue;
            UpdateText();
            onValueChanged.Invoke(value);
        }
    }

    public void Awake()
    {
        mainButton.onClick.AddListener(OnMainButtonClick);
        plusButton.onClick.AddListener(OnPlusButtonClick);
        minusButton.onClick.AddListener(OnMinusButtonClick);
        UpdateText();
    }

    private void OnMainButtonClick()
    {
        var nextBiggerValue = defaultValues.FirstOrDefault(defaultValue => defaultValue > value);
        if (nextBiggerValue != 0)
            Value = nextBiggerValue;
        else
            Value = defaultValues.Min();
    }

    private void OnPlusButtonClick()
    {
        Value += delta;
    }

    private void OnMinusButtonClick()
    {
        Value -= delta;
    }

    private void UpdateText()
    {
        mainText.text = string.Format(mainTextFormat, value);
    }
}