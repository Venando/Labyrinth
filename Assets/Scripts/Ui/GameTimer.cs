using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;

    private double startTime = 0f;
    private bool isRunning = false;

    public void StartTimer(double zeroTime)
    {
        isRunning = true;
        startTime = zeroTime;
        StartCoroutine(UpdateTimer());
    }

    private void OnEnable()
    {
        if (isRunning)
        {
            StartCoroutine(UpdateTimer());
        }
    }

    private IEnumerator UpdateTimer()
    {
        var secondWait = new WaitForSeconds(1f);
        while (isRunning)
        {
            var elapsedTime = Time.timeAsDouble - startTime + 0.1f;
            timerText.text = Mathf.CeilToInt((float)elapsedTime).ToString();
            yield return secondWait;
        }   
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
