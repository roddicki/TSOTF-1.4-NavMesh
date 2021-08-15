using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
   
    private Destroy destroy;
    public float timeRemaining;
    private bool startTimer;

    // Start is called before the first frame update
    void Start() {
        destroy = GetComponent<Destroy>();
		startTimer = true;
    }

    // Update is called once per frame 
    void Update() {
        if (TimeUp())
        {
            destroy.DestroyCubes();
            destroy.DestroyAgents();
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
