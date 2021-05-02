using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// state class
public class State
{
    public enum STATE {
		IDLE, PATROL, STOP, PURSUE, ATTACK, SlEEP
	};

	public enum EVENT {
		ENTER, UPDATE, EXIT
	};

	public STATE name;
	protected EVENT stage;
	protected GameObject npc;
	protected Animator anim;
	protected Transform player;
	protected State nextState;
	protected NavMeshAgent agent;

	float visDist = 10.0f;
	float visAngle = 30.0f;
	float shootDist = 7.0f;

	public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
	{
		npc = _npc;
		agent = _agent;
		anim = _anim;
		stage = EVENT.ENTER;
		player = _player;
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
// idle state
public class Idle: State 
{
	public Idle (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base(_npc, _agent, _anim, _player)
	{
		name = STATE.IDLE;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString());
		anim.SetTrigger ("isIdle");
		base.Enter ();
	}

	public override void Update ()
	{
		if(Random.Range(0, 100) < 10) 
		{
			nextState = new Patrol (npc, agent, anim, player);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}



//------------------------------------------//
// patrol state
public class Patrol: State 
{
	int currentIndex = -1;

	public Patrol (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base (_npc, _agent, _anim, _player)
	{
		name = STATE.PATROL;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString());
		currentIndex = 0;
		anim.SetTrigger ("isWalking");
		base.Enter ();
	}

	public override void Update ()
	{
		// conditions for moving to next state
		if (agent.remainingDistance < 1) {
			nextState = new Stop (npc, agent, anim, player);
			stage = EVENT.EXIT;
		}

		// set next way point
		if (agent.remainingDistance < 1) 
		{
			// actions
			Debug.Log ("arrived");
			//if(currentIndex >= GameEnvironment.Singleton.Checkpoints.Count - 1) 
			//{
			//	currentIndex = 0;
			//} else 
			//{
			//	currentIndex += 1;
			//}

			//agent.SetDestination (GameEnvironment.Singleton.Checkpoints [currentIndex].transform.position);

		}
	}

	public override void Exit ()
	{
		anim.ResetTrigger ("isWalking");
		base.Exit ();
	}
}

//------------------------------------------//
// stop state
public class Stop : State {
	public Stop (GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base (_npc, _agent, _anim, _player)
	{
		name = STATE.STOP;
		agent.speed = 0; //nav mesh
		agent.isStopped = true;
	}

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		anim.SetTrigger ("isIdle");
		base.Enter ();
	}

	public override void Update ()
	{
		// do nothing stay in idle state
	}

	public override void Exit ()
	{
		anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}

