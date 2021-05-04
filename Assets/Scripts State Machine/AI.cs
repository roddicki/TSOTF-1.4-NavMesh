using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
	NavMeshAgent agent;
	Animator anim;
	public Transform cube;
	public Transform bay;
	State currentState;
	private AgentHealth health;

	// Start is called before the first frame update
    void Start()
    {
		agent = this.GetComponent<NavMeshAgent> ();
		anim = this.GetComponent<Animator> ();
		health = this.GetComponent<AgentHealth> ();
		currentState = new Idle (this.gameObject, agent, anim, cube, bay, health);
    }

    // Update is called once per frame
    void Update()
    {
		currentState = currentState.Process ();
    }
}
