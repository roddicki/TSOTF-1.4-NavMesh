using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 20;
    public bool startTimer = false;

    private void Start()
    {
        // Starts the timer automatically
        //startTimer = true;
    }

    void Update()
    {
        if (startTimer)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                startTimer = false;
            }
        }
    }
}