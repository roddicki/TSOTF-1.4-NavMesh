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
    string[] agentName;

	void Start() {
		// tie the bay to an agent with the same name
		agentName = this.name.Split('-');
        //Debug.Log(agentName[0]);
        
        goal = false;
        ownGoal = false;
        goalCount = 0;
		//agentHealth = Agent.GetComponent<AgentHealth> ();
	}

	// check agent still attached
	void CheckAgentAttached()
	{
		agentName = this.name.Split('-');
		Agent = GameObject.Find (agentName [0]);
		if (Agent != null) {
			agentHealth = Agent.GetComponent<AgentHealth> ();
		}
		
	}

	void OnTriggerEnter(Collider other) {
		CheckAgentAttached();
		// Touched goal and goal name
		if (other.gameObject.CompareTag("cube")) {
            goal = true;
			agentHealth.Health += 100.0f;
            goalCount++;
        }
    }

    void OnTriggerExit(Collider other){
		CheckAgentAttached();
        if (other.gameObject.CompareTag("cube")) 
        {
            ownGoal = true;
			agentHealth.Health -= 100.0f;
        }
    }
    // change ground material when goal scored
}
