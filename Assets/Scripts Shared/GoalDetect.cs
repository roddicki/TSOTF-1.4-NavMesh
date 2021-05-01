using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetect : MonoBehaviour
{
    public bool goal;
    public int goalCount;
    public bool ownGoal;
	public GameObject Agent;
	AgentHealth agentHealth;

    void Start() {
       goal = false;
        ownGoal = false;
        goalCount = 0;
		agentHealth = Agent.GetComponent<AgentHealth> ();
    }

    void OnTriggerEnter(Collider other) {
        // Touched goal and goal name
        if (other.gameObject.CompareTag("cube")) {
            goal = true;
			agentHealth.Health += 100.0f;
            goalCount++;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("cube")) 
        {
            ownGoal = true;
			agentHealth.Health -= 100.0f;
        }
    }

    // change ground material when goal scored

}
