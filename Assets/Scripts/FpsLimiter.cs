using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    [SerializeField] private bool active = false;
    [SerializeField] private int limit = 240;
    [SerializeField] private bool vSync = true;

    private void Start()
    { 
        if (active)
        {
            Application.targetFrameRate = limit;
            QualitySettings.vSyncCount = vSync ? 1 : 0;      
            Time.fixedDeltaTime = 0.02f;
            Time.maximumDeltaTime = 0.1f; 
        }
    }
}
