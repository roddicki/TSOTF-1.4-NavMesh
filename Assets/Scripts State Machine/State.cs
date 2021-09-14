using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// state class
public class State
{
    public enum STATE {
		IDLE, SETBEHAVIOUR, ABSTAIN, SETTARGETHONEST, SETTARGET, SETTARGETSTEAL, SEARCH, PUSH, STOP, BREATHLESS, DEATH
	};

	public enum EVENT {
		ENTER, UPDATE, EXIT
	};

	public STATE name;
	protected EVENT stage;
	protected Animator anim;
	protected GameObject npc;
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


//=================================================================================================================//
// Idle state
public class Idle: State 
{
	public Idle (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.IDLE;
	}

	// get spawn script
	private Spawn spawn;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString ());
		spawn = GameObject.Find("GameManager").GetComponent<Spawn>();
		Debug.Log("IDLE and CubeSpawnComplete=" + spawn.CubeSpawnComplete);
		base.Enter ();
	}

	public override void Update ()
	{
		if( (Random.Range(0, 100) < 10) && spawn.CubeSpawnComplete) 
		{
			nextState = new SetTarget (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		base.Exit ();
	}
}

//=================================================================================================================//
// SetBehaviour state
public class SetBehaviour: State 
{
	public SetBehaviour (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SETBEHAVIOUR;
	}

	// behaviour coefficients
	private AgentBehaviour Behaviour;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString ());
		// assign behaviour
		Behaviour = npc.GetComponent<AgentBehaviour>();
		CubesCollected(health, npc);
		base.Enter ();
	}

	public override void Update ()
	{
		// set agent behaviour here

		// if Charity
		if (CubesCollected(health, npc) > 3 && Behaviour.Charity > 0.5f)
		{
			ChooseBay();
			nextState = new Abstain (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
		// Abstain if cubes collected > 3 
		else if (CubesCollected(health, npc) > 3)
		{
			nextState = new Abstain (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
		else {
			nextState = new SetTargetHonest (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

		// Steal - take cubes from another bay
		//nextState = new SetTargetSteal (npc, agent, anim, cube, bay, health);
		//stage = EVENT.EXIT;

		// RobinHood - take cunes from wealthy
		
	}

	public override void Exit ()
	{
		base.Exit ();
	}

	// get number of cubes in the agents bay // to help get the health of each agent
	int CubesCollected(AgentHealth health, GameObject npc) 
	{
		// Do this using AgentHealth 100 = 1 cube
		int cubes;
		if (health.Delay > 0.0f)
		{
			cubes = Mathf.RoundToInt( (health.Health - 200.0f)/100.0f );
		}
		else
		{
			cubes = Mathf.RoundToInt( (health.Health)/100.0f );
		}
		//Debug.Log(npc.name +" cubes: "+ cubes);
		return cubes;
	}


	// choose a bay to give aid too
	void ChooseBay()
	{
		GameObject [] agents = GameObject.FindGameObjectsWithTag("agent");
		// create list of all bays with less than 2 cubes
		// randomnly choose one and assign as this agents bay
		List<GameObject> bays = new List<GameObject>();
		// find no of cubes for each bay
		foreach (var agent in agents)
		{
			AgentHealth health = agent.GetComponent<AgentHealth> ();
			int cubes = CubesCollected(health, agent);
			Debug.Log(agent.name+ " HAS " +cubes+ " CUBES");
			// get the bay name
			GameObject bay = GameObject.Find(agent.name+ "-bay");
			// if less than 2 cubes and a different agent add to the list
			if (cubes < 2 && agent.name != npc.name)
			{
				bays.Add(bay);
			}
		}
		// choose bay to give aid too
		int chosenBay = Random.Range(0, bays.Count);
		if (bays.Count > 0)
		{
			Debug.Log(bays[chosenBay].name + " - chosen by " + npc.name);
		}
		
		foreach (var bay in bays)
		{
			Debug.Log(bay.name);
		}
	}

}

//=================================================================================================================//
// Abstain state
public class Abstain: State 
{
	public Abstain (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.ABSTAIN;
	}

	// get spawn script
	private Spawn spawn;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString ());
		// move to own bay 
		agent.SetDestination (bay.transform.position);
		base.Enter ();
	}

	public override void Update ()
	{

		// health
		if (health.Health < 105.0f) {
			nextState = new Breathless (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		} else {
			nextState = new SetBehaviour (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		base.Exit ();
	}
}


//=================================================================================================================//
// SetTargetSteal state
public class SetTargetSteal : State {

	public SetTargetSteal (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SETTARGETSTEAL;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public Collider bayCollider;
	public Collider centralBayCollider;
	public GameObject [] allBays;
	//public float timeRemaining;
	private Timer timer;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString ());
		//timeRemaining = npc.GetComponent<Timer>().timeRemaining;
		timer = npc.GetComponent<Timer>();
		// start timer - 7s to find a cube to steal
		timer.timeRemaining = 7.0f;
		timer.startTimer = true;
		// get own bay
		bayCollider = bay.GetComponent<Collider> ();
		// get all bays
		allBays = GameObject.FindGameObjectsWithTag("bay");
		// get central bay
		centralBayCollider = GameObject.Find ("Ground Decal Square").GetComponent<Collider> ();
		base.Enter ();
		// set wandering destination toward navmeshcentre
		agent.SetDestination (RandomPointInBounds (centralBayCollider.bounds));
	}

	public override void Update ()
	{
		// wander to find cube while timer running
		if (agent.pathPending != true && agent.remainingDistance < 1 && timer.timeRemaining > 0) {
			agent.SetDestination (Wander());
		} 
		// timer run out stop trying to steal and go to Set Target
		else if (timer.timeRemaining <= 0){
			Debug.Log(npc.name + "out of Time change to SETTARGET");
			timer.startTimer = false;
			// goto next state
			nextState = new SetTarget (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
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
			//Debug.DrawRay (source, angle * 40, Color.red, 1.0f);
			// got a hit
			if (Physics.SphereCast (ray, 3.0f, out hit, 60)) {
				Debug.DrawRay (source, angle * hit.distance, Color.red);
				// if hit is cube in a bay & hit cube not contained in own bay - stealing
				if (hit.collider.tag == "cube" && IsInBay(hit.collider.name) && bayCollider.bounds.Contains(GameObject.Find (hit.collider.name).transform.position) == false) 
				{
					// set as target cube
					cube = GameObject.Find (hit.collider.name);
					// start timer in Timer.cs
					timer.timeRemaining = 20;
					timer.startTimer = true;
					// goto next state
					nextState = new Search (npc, agent, anim, cube, bay, health);
					stage = EVENT.EXIT;
				}
			}
		}

		// health
		if (health.Health < 105.0f) {
			nextState = new Breathless (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isWalking");
		base.Exit ();
	}

	// find cube


	// return random point in central bay
	Vector3 RandomPointInBounds (Bounds bounds)
	{
		return new Vector3 (
			Random.Range (bounds.min.x, bounds.max.x),
			Random.Range (bounds.min.y, bounds.max.y),
			Random.Range (bounds.min.z, bounds.max.z)
		);
	}

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

	// is cube in a bay
	bool IsInBay(string cubeName)
	{
		foreach (GameObject bay in allBays)
		{
			// get collider
			Collider bayBoxCollider = bay.GetComponent<Collider>();
			// if in a bay
			if (bayBoxCollider.bounds.Contains(GameObject.Find (cubeName).transform.position))
			{
				return true;
			}
		}
		return false;
	}
}

//=================================================================================================================//
// SetTargetHonest state
public class SetTargetHonest : State {

	public SetTargetHonest (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SETTARGETHONEST;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public Collider bayCollider;
	public Collider centralBayCollider;
	public GameObject [] allBays;
	//public float timeRemaining;
	private Timer timer;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString ());
		//timeRemaining = npc.GetComponent<Timer>().timeRemaining;
		timer = npc.GetComponent<Timer>();
		// start timer - 20s to find a cube to collect
		timer.timeRemaining = 20.0f;
		timer.startTimer = true;
		// get own bay
		bayCollider = bay.GetComponent<Collider> ();
		// get all bays
		allBays = GameObject.FindGameObjectsWithTag("bay");
		// get central bay
		centralBayCollider = GameObject.Find ("Ground Decal Square").GetComponent<Collider> ();
		base.Enter ();
		// set wandering destination toward navmeshcentre
		agent.SetDestination (RandomPointInBounds (centralBayCollider.bounds));
	}

	public override void Update ()
	{
		// wander to find cube while timer running
		if (agent.pathPending != true && agent.remainingDistance < 1 && timer.timeRemaining > 0) {
			agent.SetDestination (Wander());
		} 
		// timer run out stop trying to steal and go to Set Target
		else if (timer.timeRemaining <= 0){
			Debug.Log(npc.name + "out of Time change to SETTARGET");
			timer.startTimer = false;
			// goto next state
			nextState = new SetTarget (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
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
			//Debug.DrawRay (source, angle * 40, Color.red, 1.0f);
			// got a hit
			if (Physics.SphereCast (ray, 3.0f, out hit, 60)) {
				Debug.DrawRay (source, angle * hit.distance, Color.red);
				// if hit is cube in a bay & hit cube not contained in own bay - stealing
				if (hit.collider.tag == "cube" && IsInBay(hit.collider.name) == false && bayCollider.bounds.Contains(GameObject.Find (hit.collider.name).transform.position) == false) 
				{
					// set as target cube
					cube = GameObject.Find (hit.collider.name);
					// start timer in Timer.cs
					timer.timeRemaining = 20;
					timer.startTimer = true;
					// goto next state
					nextState = new Search (npc, agent, anim, cube, bay, health);
					stage = EVENT.EXIT;
				}
			}
		}

		// health
		if (health.Health < 105.0f) {
			nextState = new Breathless (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isWalking");
		base.Exit ();
	}

	// find cube


	// return random point in central bay
	Vector3 RandomPointInBounds (Bounds bounds)
	{
		return new Vector3 (
			Random.Range (bounds.min.x, bounds.max.x),
			Random.Range (bounds.min.y, bounds.max.y),
			Random.Range (bounds.min.z, bounds.max.z)
		);
	}

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

	// is cube in a bay
	bool IsInBay(string cubeName)
	{
		foreach (GameObject bay in allBays)
		{
			// get collider
			Collider bayBoxCollider = bay.GetComponent<Collider>();
			// if in a bay
			if (bayBoxCollider.bounds.Contains(GameObject.Find (cubeName).transform.position))
			{
				return true;
			}
		}
		return false;
	}
}


//=================================================================================================================//
// SetTarget state
public class SetTarget : State {

	public SetTarget (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SETTARGET;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}

	public Collider bayCollider;
	public Collider centralBayCollider;
	public float timeRemaining;
	// behaviour
	private AgentBehaviour agentBehaviour;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString ());
		timeRemaining = npc.GetComponent<Timer>().timeRemaining;
		bayCollider = bay.GetComponent<Collider> ();
		// get central bay
		centralBayCollider = GameObject.Find ("Ground Decal Square").GetComponent<Collider> ();
		base.Enter ();
		// set wandering destination toward navmeshcentre
		agent.SetDestination (RandomPointInBounds (centralBayCollider.bounds));
		// assign behaviour
		agentBehaviour = npc.GetComponent<AgentBehaviour>();
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
			//Debug.DrawRay (source, angle * 40, Color.red, 1.0f);
			// got a hit
			if (Physics.SphereCast (ray, 3.0f, out hit, 60)) {
				Debug.DrawRay (source, angle * hit.distance, Color.red);
				// if hit cube & hit cube not contained in own bay
				if (hit.collider.tag == "cube" && bayCollider.bounds.Contains(GameObject.Find (hit.collider.name).transform.position) == false) 
				{
					Debug.Log (hit.collider.name);
					// set as target cube
					cube = GameObject.Find (hit.collider.name);
					// start timer in Timer.cs
					npc.GetComponent<Timer>().timeRemaining = 20;
					npc.GetComponent<Timer>().startTimer = true;
					// goto next state
					nextState = new Search (npc, agent, anim, cube, bay, health);
					stage = EVENT.EXIT;
				}
			}
		}

		// health
		if (health.Health < 105.0f) {
			nextState = new Breathless (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isWalking");
		base.Exit ();
	}

	// find cube


	// return random point in central bay
	Vector3 RandomPointInBounds (Bounds bounds)
	{
		return new Vector3 (
			Random.Range (bounds.min.x, bounds.max.x),
			Random.Range (bounds.min.y, bounds.max.y),
			Random.Range (bounds.min.z, bounds.max.z)
		);
	}

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

//=================================================================================================================//
// Search state - found a target moving to it
public class Search: State 
{
	public Search (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.SEARCH;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
	}
	public NavMeshObstacle navMeshObstacle;
	// timer variable
	public float timeRemaining;

	public override void Enter ()
	{
		Debug.Log (npc.name + " " + name.ToString());
		agent.speed = 4;
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

		// if still trying to get to cube give up after timer ends
		timeRemaining = npc.GetComponent<Timer>().timeRemaining;
		if (timeRemaining == 0)
		{
			nextState = new SetBehaviour (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

		// slow down
		if (agent.pathPending != true && agent.remainingDistance < 4) {
			agent.speed = 2;
		}

		// health
		if (health.Health < 105.0f) {
			nextState = new Breathless (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isWalking");
		base.Exit ();
	}
}

//=================================================================================================================//
// push state
public class Push : State {


	public Push (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.PUSH;
		agent.speed = 4; //nav mesh
		agent.isStopped = false;
	}

	public NavMeshObstacle navMeshObstacle;
	// timer variable
	public float timeRemaining;

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		navMeshObstacle = cube.GetComponent<NavMeshObstacle> ();
		navMeshObstacle.enabled = false;
		//anim.SetTrigger ("isIdle");
		// set destination to bay.position
		agent.SetDestination (bay.transform.position);
		agent.speed = 4;
		base.Enter ();
	}

	public override void Update ()
	{
		// conditions for moving to next state
		// if still trying to get to cube give up after timer ends
		timeRemaining = npc.GetComponent<Timer>().timeRemaining;
		if (timeRemaining == 0)
		{
			nextState = new SetBehaviour (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

		// if path resolved and agent has moved to target
		if (agent.pathPending != true && agent.remainingDistance < 1) {
			//navMeshObstacle.enabled = true; // enable so agent avoids in the bay
			nextState = new Stop (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		} 
		// else if agent loses cube
		else if (Vector3.Distance (cube.transform.position, agent.transform.position) >= 2.5f) 
		{
			nextState = new Search (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}

		// slow down
		if (agent.pathPending != true && agent.remainingDistance < 2) {
			agent.speed = 2;
		}

		// health
		if (health.Health < 105.0f) {
			nextState = new Breathless (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}

}


//=================================================================================================================//
// stop state
public class Stop : State {
	public Stop (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base(_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.STOP;
		agent.speed = 2; //nav mesh
		agent.isStopped = false;
		// stop.. doesn't work
		//agent.SetDestination (Vector3.zero);
		// reset Timer in Timer.cs
		npc.GetComponent<Timer>().timeRemaining = 20; // DO I NEED THIS
	}

	public NavMeshObstacle navMeshObstacle;
	// timer variable
	public float timeRemaining;

	public override void Enter ()
	{
		Debug.Log (name.ToString ());
		navMeshObstacle = cube.GetComponent<NavMeshObstacle> ();
		navMeshObstacle.enabled = true;
		//anim.SetTrigger ("isIdle");
		base.Enter ();
	}

	public override void Update ()
	{
		nextState = new SetBehaviour (npc, agent, anim, cube, bay, health);
		stage = EVENT.EXIT;
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}

//=================================================================================================================//
// Breathless state
public class Breathless : State {
	public Breathless (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.BREATHLESS;
		agent.speed = 0; //nav mesh
		agent.isStopped = true;
		agent.SetDestination (Vector3.zero);
	}

	public int PantingPose;

	public override void Enter ()
	{
		Debug.Log (name.ToString () + " " + npc.name);
		PantingPose = Random.Range(0,3);
		anim.SetInteger("PantingInt", PantingPose);
		anim.SetBool("IsPanting", true);
		base.Enter ();
	}

	public override void Update ()
	{
		
		// health 
		if (health.Health > 200.0f) {
			anim.SetBool("IsPanting", false);
			agent.isStopped = false;
			agent.speed = 2;
			nextState = new Search (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		} 
		else if (health.Health < 10.0f) {
			int DeathPose = Random.Range(0,1);
			anim.SetInteger("ProneInt", DeathPose);
			anim.SetBool("IsDeath", true);
			agent.isStopped = true;
			agent.speed = 0;
			nextState = new Death (npc, agent, anim, cube, bay, health);
			stage = EVENT.EXIT;
		}
		
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}
}

//=================================================================================================================//
// Death state
public class Death : State {
	public Death (GameObject _npc, NavMeshAgent _agent, Animator _anim, GameObject _cube, GameObject _bay, AgentHealth _health) : base (_npc, _agent, _anim, _cube, _bay, _health)
	{
		name = STATE.DEATH;
		agent.speed = 0; //nav mesh
		agent.isStopped = true;
		agent.SetDestination (Vector3.zero);
	}

	// timer variables
	private float timeRemaining;
	private bool startTimer;
	private GameObject m_Ragdoll;
	// death color script - to run a coroutine
	private DeathColor deathColor;

	public override void Enter ()
	{
		Debug.Log (name.ToString () + " " + npc.name);
		base.Enter ();
		// delay ragdoll
		timeRemaining = Random.Range(2.0f, 15.0f);
		startTimer = true;
		deathColor = npc.GetComponent<DeathColor> ();
	}

	public override void Update ()
	{
		// timer delays ragdoll activation
		if (Timer()) {
			ActivateRagdoll ();
			deathColor.StartCoroutine (deathColor.IniateDeathColor (m_Ragdoll, npc.name));
		}
		//nextState = new Idle (npc, agent, anim, cube, bay, health);
		//stage = EVENT.EXIT;
	}

	public override void Exit ()
	{
		//anim.ResetTrigger ("isIdle");
		base.Exit ();
	}


	// timer
	private bool  Timer()
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

	// activate ragdoll
	private void ActivateRagdoll() 
	{
		// get ragdoll
		m_Ragdoll = agent.transform.GetChild(1).gameObject; 
		if (m_Ragdoll.tag != "ragdoll") {
			Debug.LogWarning("Ragdoll may not be present as a child of the agent.");
            return;
		}
		// get agent model
		GameObject m_AgentModel = agent.transform.Find("shadow_human_remodelled-5").gameObject;
		if (m_AgentModel == null) {
            Debug.LogWarning("model may not be present as a child of the agent.");
            return;
        }
		// copy position
		CopyTransformData(m_AgentModel.transform, m_Ragdoll.transform);
		// activate / deactivate
		agent.GetComponent<CapsuleCollider>().enabled = false;
		m_Ragdoll.gameObject.SetActive(true);
		agent.GetComponent<Animator>().enabled = false;
		m_AgentModel.gameObject.SetActive(false);
		agent.GetComponent<NavMeshAgent>().enabled = false;
		m_Ragdoll.transform.parent = null;
	}

	// copy agent position to ragdoll
	private void CopyTransformData(Transform sourceTransform, Transform destinationTransform)
    {
        if (sourceTransform.childCount != destinationTransform.childCount)
        {
            Debug.LogWarning("Invalid transform copy, they need to match transform hierarchies");
            return;
        }

        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            var source = sourceTransform.GetChild(i);
            var destination = destinationTransform.GetChild(i);
            destination.position = source.position;
            destination.rotation = source.rotation;
            var rb = destination.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;
            CopyTransformData(source, destination);
        }
    }



}


