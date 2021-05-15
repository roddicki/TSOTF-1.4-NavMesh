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
	private bool AgentNotAttached;


	void Start() {
		// tie the bay to an agent with the same name
		AgentNotAttached = true;
		agentName = this.name.Split('-');
        //Debug.Log(agentName[0]);
        
        goal = false;
        ownGoal = false;
        goalCount = 0;
		//agentHealth = Agent.GetComponent<AgentHealth> ();
	}

	private void Update ()
	{
		// only GetComponent when Agent has been instantiated
		if (AgentNotAttached && GameObject.Find (agentName [0]) != null) {
			Agent = GameObject.Find (agentName [0]);
			agentHealth = Agent.GetComponent<AgentHealth> ();
			AgentNotAttached = false;
		}
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
