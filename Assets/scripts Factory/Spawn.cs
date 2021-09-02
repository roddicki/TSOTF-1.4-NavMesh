using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour{
	public bool DoSpawn;

	// cube
	public GameObject SpawnedCube;
	private float SpawnWait;
	public int NumCubes;
	public float SpawnPosMaxX;
	public float SpawnPosMinX;
	public float SpawnPosMaxZ;
	public float SpawnPosMinZ;
	public bool CubeSpawnComplete;

	// agent
	public GameObject AgentToSpawn;
	public GameObject ModelToSpawn;
	// ragdoll
	public GameObject Ragdoll;
	public bool AgentSpawnComplete;
	// marker color
	// Dictionary<keytype, objectType> dictionaryName;
    Dictionary <string, Color> markerColors = new Dictionary<string, Color>();

	// Start is called before the first frame update
	void Awake() {
		GetMarkerColor();
		SpawnWait = 0.01F;
		if (DoSpawn){
			StartCoroutine (AgentSpawner ());
			//StartCoroutine (RagollSpawner ()); // ragdolls are spawned from AgentSpawner
			StartCoroutine (CubeSpawner ());
		}

		CubeSpawnComplete = false;
		AgentSpawnComplete = false;

	}

	// create list of bay names and marker colors
	private void GetMarkerColor() {
		GameObject [] bays = GameObject.FindGameObjectsWithTag("bay");
		foreach (GameObject bay in bays)
		{
			// get marker material
			GameObject marker = bay.transform.Find ("marker").gameObject; 
			Renderer m_MarkerRenderer = marker.GetComponent<Renderer> ();
			Material m_MarkerMaterial = m_MarkerRenderer.material;
			Debug.Log(m_MarkerMaterial.color);
			// add to dictionary
			markerColors.Add(bay.name, m_MarkerMaterial.color);
		}   
	}

	// spawn an agent for each bay
	public IEnumerator AgentSpawner () {
		// get the gameobject / agent to spawn / model from the prefab in the public variable
		GameObject[] bays = GameObject.FindGameObjectsWithTag ("bay");
		int i = 0;
		foreach (GameObject bay in bays) {
			Vector3 SpawnPos = bay.transform.position;
			GameObject NewAgent = Instantiate (AgentToSpawn, SpawnPos, AgentToSpawn.transform.rotation);
			NewAgent.name = "Agent" + i;

			// set marker material
			GameObject marker = bay.transform.Find ("marker").gameObject; 
			Renderer m_MarkerRenderer = marker.GetComponent<Renderer> ();
			m_MarkerRenderer.material.color = markerColors[bay.name];
			Debug.Log(bay.name +" "+ markerColors[bay.name] +" "+ NewAgent.name);
			// set material of the model shadow_human_rigged_001_geo that is a child of agent
			GameObject m_RiggedAgent = NewAgent.transform.GetChild(0).gameObject.transform.Find (ModelToSpawn.name).gameObject; // works
			Renderer m_RiggedAgentRenderer = m_RiggedAgent.GetComponent<SkinnedMeshRenderer> ();
			m_RiggedAgentRenderer.material.color = markerColors[bay.name]; 
			i += 1;
			yield return null;
		}
		Debug.Log ("==Agent Spawn Complete==");
		AgentSpawnComplete = true;
		// move this!!!!!
		StartCoroutine (RagollSpawner ());
	}


	// spawn a ragdoll for each agent
	public IEnumerator RagollSpawner(){
		// find all agents
		GameObject[] agents = GameObject.FindGameObjectsWithTag ("agent");
		int i = 0;
		foreach (GameObject agent in agents){
			Vector3 SpawnPos = agent.transform.position;
			// Instantiate at ragdoll.
			// To do make child of agent
        	GameObject NewRagdoll = Instantiate(Ragdoll, SpawnPos, agent.transform.rotation, agent.transform);
			// change colour of ragdoll
			// get colour from agent
			Renderer m_AgentRenderer = agent.transform.GetChild(0).gameObject.transform.Find("shadow_human_rigged_001_geo").GetComponent<Renderer> ();
			Material m_AgentMaterial = m_AgentRenderer.material;
			// apply material 
			GameObject m_Ragdoll = NewRagdoll.transform.Find ("shadow_human_rigged_001_geo").gameObject; // works
			Renderer m_RagdollRenderer = m_Ragdoll.GetComponent<SkinnedMeshRenderer> ();
			m_RagdollRenderer.material = m_AgentMaterial; 
			// set ragdoll tag to find it later
			NewRagdoll.tag = "ragdoll";
			NewRagdoll.gameObject.SetActive(false);
			yield return null;
		}
	}


	// spawn cubes
	public IEnumerator CubeSpawner (){
		//float StartPosX = Random.Range(SpawnPosMinX, SpawnPosMaxX);
		//float StartPosZ = Random.Range(SpawnPosMinZ, SpawnPosMaxZ);
		float StartPosX = SpawnPosMinX;
		float StartPosZ = SpawnPosMinZ;
		float Xinc = 0;
		float Zinc = 0;
		float Y = 2;
		for (int i = 0; i < NumCubes; i++) {
			//Debug.Log ("Creating cube number: " + i);
			//set range of X and Z positions
			Vector3 SpawnPos = new Vector3(StartPosX + Xinc, Y, StartPosZ + Zinc);
			Xinc = Xinc + Random.Range(1.01F, 1.3F);
			if(StartPosX + Xinc > SpawnPosMaxX) {
				StartPosX = SpawnPosMinX;
				Xinc = 0;
				Zinc = Zinc + Random.Range(1.01F, 1.2F);
			}
			if(StartPosZ + Zinc > SpawnPosMaxZ) {
				StartPosZ = SpawnPosMinZ;
				Zinc = 0;
				Y++;
			}
			//Vector3 SpawnPos = new Vector3(Random.Range(SpawnPosMinX, SpawnPosMaxX), 5, Random.Range(SpawnPosMinZ, SpawnPosMaxZ));
			GameObject NewCube = Instantiate (SpawnedCube, SpawnPos, this.transform.rotation);
			NewCube.name = "Cube" + i;
			NewCube.layer = 0;
			yield return new WaitForSeconds (SpawnWait);
		}
		Debug.Log("==Cube Spawn Complete==");
		CubeSpawnComplete = true;
	}
}
