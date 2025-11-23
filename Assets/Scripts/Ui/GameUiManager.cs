using System;
using System.Collections.Generic;
using UnityEngine;


public class GameUiManager : MonoBehaviour
{
    [SerializeField] private UiWindowObject baseActiveWindow;

    private UiWindowObject[] windows;

    private Stack<UiWindowObject> windowsStack = new();
    
    private void Awake()
    {
        InitiateWindowsArray();
        ResetToDefault();
    }

    private void InitiateWindowsArray()
    {
        windows = GetComponentsInChildren<UiWindowObject>(true);
    }

    public void DisableAll()
    {
        foreach (UiWindowObject window in windows)
            window.gameObject.SetActive(false);
    }
    
    public T Open<T>() where T : UiWindowObject
    {
        DisableAll();
        T window = (T)Array.Find(windows, w => w.GetType() == typeof(T));
        window.gameObject.SetActive(true);
        windowsStack.Push(window);
        return window;
    }

    public void CloseLast()
    {
        if (!windowsStack.TryPop(out var closeWindow))
            return;

        closeWindow.gameObject.SetActive(false);

        if (!windowsStack.TryPeek(out var openWindow))
            return;

        openWindow.gameObject.SetActive(true);
    }

    public void ResetToDefault()
    {
        DisableAll();
        windowsStack.Clear();
        if (baseActiveWindow != null)
        {
            windowsStack.Push(baseActiveWindow);
            baseActiveWindow.gameObject.SetActive(true);
        }
    }

    public T Get<T>() where T : UiWindowObject
    {
        if (windows == null || windows.Length == 0)
            InitiateWindowsArray();

        var window = Array.Find(windows, w => w.GetType() == typeof(T));
        return (T)window;
    }
}