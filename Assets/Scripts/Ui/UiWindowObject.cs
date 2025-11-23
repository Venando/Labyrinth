using UnityEngine;

public class UiWindowObject : MonoBehaviour
{
    protected GameUiManager UiManager;
    
    protected virtual void Awake()
    {
        UiManager = GetComponentInParent<GameUiManager>();
    }
}