using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
   
    public Destroy destroy;
    public Spawn spawn;
    public float timeAllowed;
    public float timeRemaining;
    private bool startTimer;

    // Start is called before the first frame update
    void Start() {
        timeRemaining = timeAllowed;
		startTimer = true;
    }

    // Update is called once per frame 
    void Update() {
        if (TimeUp())
        {
            destroy.DestroyCubes();
            destroy.DestroyAgents();
            StartCoroutine (spawn.AgentSpawner ());
			StartCoroutine (spawn.CubeSpawner ());
            timeRemaining = timeAllowed;
		    startTimer = true;
        }
       //Timer();
    }

    // timer
	private bool TimeUp()
	{
		if (startTimer) {
            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
				return false;
            }
            else {
                timeRemaining = 0;
                startTimer = false;
				return true;
            }
        } 
		else {
			return false;
		}
	}
}
