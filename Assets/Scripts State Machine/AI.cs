using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
	public GameObject npc;
	NavMeshAgent agent;
	Animator anim;
	private GameObject cube;
	public GameObject bay;
	public GameObject currentBay;
	public State currentState;
	private AgentHealth health;

	// Start is called before the first frame update
    void Start()
    {
		npc = gameObject;
		bay = GameObject.Find(this.name + "-bay");
		agent = this.GetComponent<NavMeshAgent> ();
		anim = this.GetComponent<Animator> ();
		health = this.GetComponent<AgentHealth> ();
		currentState = new Idle (npc, agent, anim, cube, bay, health);
    }

    // Update is called once per frame
    void Update()
    {
		currentState = currentState.Process ();
    }

	
}
