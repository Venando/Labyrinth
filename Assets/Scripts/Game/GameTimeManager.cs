using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour, ISavable
{
    public string SaveKey => gameObject.name;

    private double startTime;

    public void StartTime()
    {
        startTime = Time.timeAsDouble;
    }

    public double GetTimeSinceGameStart()
    {
        return Time.timeAsDouble - startTime;
    }

    public double GetGameStartTime()
    {
        return startTime;
    }

    public int GetTimeSinceGameStartInSeconds()
    {
        return Mathf.CeilToInt((int)GetTimeSinceGameStart());
    }

    public string CaptureState()
    {
        return JsonUtility.ToJson(new GameTimeState { timeSinceStart = GetTimeSinceGameStart() });
    }

    public void RestoreState(string json)
    {
        var gameTimeState = JsonUtility.FromJson<GameTimeState>(json);
        startTime = Time.timeAsDouble - gameTimeState.timeSinceStart;
    }

    [Serializable]
    public class GameTimeState
    {
        public double timeSinceStart;
    }
}
