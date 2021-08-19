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
	public bool AgentSpawnComplete;

	// ragdoll
	public GameObject Ragdoll;


	// Start is called before the first frame update
	void Awake() {
		SpawnWait = 0.01F;
		if (DoSpawn){
			StartCoroutine (AgentSpawner ());
			//StartCoroutine (RagollSpawner ());
			StartCoroutine (CubeSpawner ());
		}

		CubeSpawnComplete = false;
		AgentSpawnComplete = false;

	}

    // Update is called once per frame
    void Update() {
        
    }

	// spawn an agent for each bay
	IEnumerator AgentSpawner (){
		// get location of each bay
		GameObject[] bays = GameObject.FindGameObjectsWithTag ("bay");
		int i = 0;
		foreach (GameObject bay in bays) {
			Debug.Log (bay.name + " - " + bay.transform.position);
			Vector3 SpawnPos = bay.transform.position;
			if (i != 0) {
				GameObject NewAgent = Instantiate (AgentToSpawn, SpawnPos, AgentToSpawn.transform.rotation);
				NewAgent.name = "Agent" + i;
				// get marker material
				GameObject marker = bay.transform.Find ("marker").gameObject; 
				Renderer m_MarkerRenderer = marker.GetComponent<Renderer> ();
				Material m_MarkerMaterial = m_MarkerRenderer.material;
				// apply material to the model shadow_human_rigged_001_geo that is a child of agent
				GameObject m_RiggedAgent = NewAgent.transform.GetChild(0).gameObject.transform.Find (ModelToSpawn.name).gameObject; // works
				Renderer m_RiggedAgentRenderer = m_RiggedAgent.GetComponent<SkinnedMeshRenderer> ();
				m_RiggedAgentRenderer.material = m_MarkerMaterial; 
			}
			i += 1;
			yield return null;
		}
		Debug.Log ("==Agent Spawn Complete==");
		AgentSpawnComplete = true;
		// move this!!!!!
		StartCoroutine (RagollSpawner ());
	}


	// spawn a ragdoll for each agent
	IEnumerator RagollSpawner(){
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
			Renderer m_AgentRenderer = agent.transform.GetChild(0).gameObject.transform.Find (ModelToSpawn.name).GetComponent<Renderer> ();
			Material m_AgentMaterial = m_AgentRenderer.material;
			// apply material 
			GameObject m_Ragdoll = NewRagdoll.transform.Find (ModelToSpawn.name).gameObject; // works
			Renderer m_RagdollRenderer = m_Ragdoll.GetComponent<SkinnedMeshRenderer> ();
			m_RagdollRenderer.material = m_AgentMaterial; 
			// set ragdoll tag to find it later
			NewRagdoll.tag = "ragdoll";
			NewRagdoll.gameObject.SetActive(false);
			yield return null;
		}
	}


	// spawn cubes
	IEnumerator CubeSpawner (){
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
			yield return new WaitForSeconds (SpawnWait);
		}
		Debug.Log("==Cube Spawn Complete==");
		CubeSpawnComplete = true;
	}
}
