using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsTracker : MonoBehaviour
{
    public SceneController controller;
    public BattleController battleC;


    public int openedChests;
    public int openedWalls;
    public int battles;
    public int bosses;
    public int totalCombos;
    public int totalGold;
    public int totalXP;

    public float elapsedTime = 0f; 
    private bool isPaused = false; 
    private bool isTimerRunning = false; 

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        elapsedTime = 0f;
        isPaused = false;
    }

    void Update()
    {
        if (isTimerRunning && !isPaused)
        {
            elapsedTime += Time.deltaTime; 
        }
    }
}



