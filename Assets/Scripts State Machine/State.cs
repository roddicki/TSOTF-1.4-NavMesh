using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// state class
public class State
{
    public enum STATE {
		IDLE, SOCIAL, SETTARGET, SEARCH, PUSH, STOP, BREATHLESS, DEATH
	};

	public enum EVENT {
		ENTER, UPDATE, EXIT
	};

	public STATE name;
	protected EVENT stage;
	protected GameObject npc;
	protected Animator anim;
	protected Transform cube;
	protected Transform bay;
	protected AgentHealth health;
	protected State nextState;
	protected NavMeshAgent agent;

	public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _cube, Transform _bay, AgentHealth _health)
	{
		npc = _npc;
		agent = _agent;
		anim = _anim;
		stage = EVENT.ENTER;
		cube = _cube;
		bay = _bay;
		health = _health;
	}

	// each event in the state
	public virtual void Enter() { stage = EVENT.UPDATE; }
	public virtual void Update() { stage = EVENT.UPDATE; }
	public virtual void Exit () { stage = EVENT.EXIT; }

	// move from one state to another
	public State Process()
	{
		if (stage == EVENT.ENTER) Enter ();
		if (stage == EVENT.UPDATE) Update();
		if (stage == EVENT.EXIT) 
		{
			Exit ();
			return nextState;
		}
		return this;
	}
}


//------------------------------------------//
// Idle state
public class Idle: State 
{
	public Idle (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _cube, Transform _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.IDLE;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString());
		//anim.SetTrigger ("isIdle");
		base.Enter ();
	}

	public override void Update ()
	{
		if(Random.Range(0, 100) < 10) 
		{
			nextState = new SetTarget (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}

//------------------------------------------//
// SetTarget state
public class SetTarget : State {
	int i = 0;
	
	public SetTarget (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _cube, Transform _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SETTARGET;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		base.Enter ();
		// set wandering destination
		agent.SetDestination (Wander ());
	}

	public override void Update ()
	{
		// wander
		if (agent.pathPending != true && agent.remainingDistance < 1) {
			agent.SetDestination (Wander());
		}


		// find cube
		RaycastHit hit;
		Vector3 source = new Vector3 (agent.transform.position.x, agent.transform.position.y + 1.5f, agent.transform.position.z);
		Vector3 angle = agent.transform.TransformDirection (Vector3.forward);
		i += 1;
		if (i == 1) {
			angle = agent.transform.TransformDirection (Vector3.forward);
			//angle = agent.transform.TransformDirection ((Vector3.forward + Vector3.right).normalized);
		} else if (i == 2) {
			angle = agent.transform.TransformDirection (Vector3.forward);
		} else if (i == 3) {
			angle = agent.transform.TransformDirection (Vector3.forward);
			//angle = agent.transform.TransformDirection ((Vector3.forward - Vector3.right).normalized);
			i = 0;
		}
		Ray ray = new Ray (source, angle);
		if (Physics.SphereCast (ray, 3.0f, out hit, 60)) {
			Debug.DrawRay (source, angle * hit.distance, Color.red);
			Debug.Log (hit.collider.name);
			if (hit.collider.tag == "cube") {
				Debug.Log (hit.collider.name);
				cube.position = GameObject.Find (hit.collider.name).transform.position;
				// cube obstacle enabled = false // so it can be pushed
				Debug.Log("NavMeshObstacle:" + cube.GetComponent<NavMeshObstacle>().enabled);
				cube.GetComponent<NavMeshObstacle>().enabled = false;
				Debug.Log("NavMeshObstacle:" + cube.GetComponent<NavMeshObstacle>().enabled);
				// goto next state
				nextState = new Search (npc, agent, anim, cube, bay, health);
				stage = EVENT.EXIT;
			}
		}
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isWalking");
		base.Exit ();
	}

	// find cube


	// return random position to move to
	Vector3 Wander()
	{
		Vector3 randomDirection = Random.insideUnitSphere * 10;
		randomDirection += agent.transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition (randomDirection, out hit, 10, 1);
		Vector3 finalPosition = hit.position;
		return finalPosition;
	}
}

//------------------------------------------//
// Search state
public class Search: State 
{
	public Search (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _cube, Transform _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SEARCH;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString());
		// set destination just behind cube
		agent.SetDestination (cube.position  - (bay.position - cube.position).normalized);
		base.Enter ();
	}

	public override void Update ()
	{
		// set destination / target
		// conditions for moving to next state
		// if path resolved and agent has moved to target
		if (agent.pathPending != true && agent.remainingDistance < 1) {
			nextState = new Push (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isWalking");
		base.Exit ();
	}
}

//------------------------------------------//
// push state
public class Push : State {


	public Push (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _cube, Transform _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.PUSH;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		//anim.SetTrigger ("isIdle");
		// set destination to bay.position
		agent.SetDestination (bay.position);
		base.Enter ();
	}

	public override void Update ()
	{
		// conditions for moving to next state
		// if path resolved and agent has moved to target
		if (agent.pathPending != true && agent.remainingDistance < 1) {
			nextState = new Stop (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}


}


//------------------------------------------//
// stop state
public class Stop : State {
	public Stop (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _cube, Transform _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.STOP;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
		// stop.. doesn't work
		//agent.SetDestination (Vector3.zero);
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		//anim.SetTrigger ("isIdle");
		base.Enter ();
	}

	public override void Update ()
	{
		// do nothing stay in idle state
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}

