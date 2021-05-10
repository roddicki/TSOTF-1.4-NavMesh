﻿using System.Collections;
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
	protected GameObject cube;
	protected GameObject bay;
	protected AgentHealth health;
	protected State nextState;
	protected NavMeshAgent agent;

	public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health)
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
	public Idle (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
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

	public SetTarget (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SETTARGET;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public Collider bayCollider;

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		bayCollider = bay.GetComponent<Collider> ();
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
		Vector3 [] angles = new Vector3 [3];

		angles [0] = agent.transform.TransformDirection ((Vector3.forward + Vector3.right).normalized);
		angles [1] = agent.transform.TransformDirection (Vector3.forward);
		angles [2] = agent.transform.TransformDirection ((Vector3.forward - Vector3.right).normalized);

		foreach (Vector3 angle in angles) {
			Ray ray = new Ray (source, angle);
			Debug.DrawRay (source, angle * 40, Color.red, 1.0f);
			if (Physics.SphereCast (ray, 3.0f, out hit, 60)) {
				Debug.DrawRay (source, angle * hit.distance, Color.red);
				// if hit cube & hit cube not contained in bay
				if (hit.collider.tag == "cube" && bayCollider.bounds.Contains(GameObject.Find (hit.collider.name).transform.position) == false) 
				{
					Debug.Log (hit.collider.name);
					// set as target cube
					cube = GameObject.Find (hit.collider.name);
					// goto next state
					nextState = new Search (npc, agent, anim, cube, bay, health);
					stage = EVENT.EXIT;
				}
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
	public Search (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SEARCH;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}
	public NavMeshObstacle navMeshObstacle;

	public override void Enter ()
	{
		Debug.Log (name.ToString());
		navMeshObstacle = cube.GetComponent<NavMeshObstacle> ();
		navMeshObstacle.enabled = true;
		base.Enter ();
	}

	public override void Update ()
	{
		// set destination / target
		// set destination just behind cube
		agent.SetDestination (cube.transform.position  - ((bay.transform.position - cube.transform.position).normalized * 2));
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


	public Push (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.PUSH;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public NavMeshObstacle navMeshObstacle;

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		navMeshObstacle = cube.GetComponent<NavMeshObstacle> ();
		navMeshObstacle.enabled = false;
		//anim.SetTrigger ("isIdle");
		// set destination to bay.position
		agent.SetDestination (bay.transform.position);
		base.Enter ();
	}

	public override void Update ()
	{
		// conditions for moving to next state
		// if path resolved and agent has moved to target
		if (agent.pathPending != true && agent.remainingDistance < 1) {
			navMeshObstacle.enabled = true; // enable so agent avoids in the bay
			nextState = new Stop (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		} 
		// else if agent loses cube
		else if (Vector3.Distance (cube.transform.position, agent.transform.position) >= 3.5f) 
		{
			nextState = new Search (npc, agent, anim, cube, bay, health);
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
	public Stop (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
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
		nextState = new SetTarget (npc, agent, anim, cube, bay, health);
		stage = EVENT.EXIT;
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}

