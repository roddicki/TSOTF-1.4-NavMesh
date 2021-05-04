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
	}

	public override void Update ()
	{
		// set destination / target
		// find cube
		RaycastHit hit;
		Vector3 source = new Vector3 (agent.transform.position.x, agent.transform.position.y + 0.5f, agent.transform.position.z);
		Vector3 angle = Vector3.forward;
		i += 1;
		if (i == 1) {
			angle = (Vector3.forward + Vector3.right).normalized;
		} else if (i == 2) {
			angle = Vector3.forward;
		} else if (i == 3) {
			angle = (Vector3.forward - Vector3.right).normalized;
			i = 0;
		}
		Ray ray = new Ray (source, angle);
		if (Physics.Raycast (ray, out hit, 40)) {
			Debug.DrawRay (source, angle * hit.distance, Color.red);
			Debug.Log (hit.collider.name);
			if (hit.collider.tag == "cube") {
				Debug.Log (hit.collider.name);
				cube.position = GameObject.Find (hit.collider.name).transform.position;
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
		agent.SetDestination (cube.position);
		base.Enter ();
	}

	public override void Update ()
	{
		// set destination / target
		// conditions for moving to next state
		// if path resolved and agent has moved to target
		if (agent.pathPending != true && agent.remainingDistance < 1) {
			nextState = new Stop (npc, agent, anim, cube, bay, health);
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

