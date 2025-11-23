using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    [SerializeField] private bool active = false;

    private void Start()
    { 
        if (active)
        {
            Application.targetFrameRate = 30;       
        }
    }
}
