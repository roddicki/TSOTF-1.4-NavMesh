using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentHealth : MonoBehaviour
{
	public float Health;
	public float Delay;
	private bool CountDown;
	private float DelayTimerMuliplier;
	public bool SuspendHealth;


	// general approach:
	// agent begins with 200points
	// a countdown timer of between 10 and 30 seconds gives the agents between 10 and 30 seconds with those 200 points
	// at t-10s the agent starts to lose those points
	// at t-0s the points have been lost
	// to survive the agent needs to replenish the points by collecting cubes (100points per cube)
	// if at any time the agents score drops below 200 points they go into isPanting mode
	// if at any time the agents score drops below 100 points they go into dying mode
	private void Start ()
	{
		Health = 0f; //205.0f;
		// get a random time limit before points are lost
		Delay = Random.Range (50.0f, 90.0f); 
		CountDown = true;
		DelayTimerMuliplier = 20.0f;
	}

	void Update ()
	{
		if (CountDown && SuspendHealth == false) {
			DelayTimer ();
		}
	}

	// countdown timer for launch 
	void DelayTimer ()
	{
		Delay -= Time.deltaTime;
		if (Delay < 10.0f) {
			// lose points every sec
			Health -= Time.deltaTime * DelayTimerMuliplier;
		}
		if (Delay <= 0.0f) {
			// stop countdown 
			CountDown = false;
		}
	}

	// reset the timer on respawn - not used yet
	public void ResetDelayTimer()
	{
		Delay = Random.Range (10.0f, 30.0f);
		CountDown = true;
	}

}
