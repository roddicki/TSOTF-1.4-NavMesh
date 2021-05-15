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
	public GameObject SpawnedAgent;
	public bool AgentSpawnComplete;


	// Start is called before the first frame update
	void Awake() {
		SpawnWait = 0.01F;
		if (DoSpawn){
			StartCoroutine (AgentSpawner ());
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
			Debug.Log (bay.transform.position);
			Vector3 SpawnPos = bay.transform.position;
			if (i != 0) {
				GameObject NewAgent = Instantiate (SpawnedAgent, SpawnPos, SpawnedAgent.transform.rotation);
				NewAgent.name = "Agent" + i;
				// get marker material
				GameObject marker = bay.transform.Find ("marker").gameObject; 
				Renderer m_MarkerRenderer = marker.GetComponent<Renderer> ();
				Material m_MarkerMaterial = m_MarkerRenderer.material;
				// apply to child of agent
				GameObject m_RiggedAgent = NewAgent.transform.Find ("shadow_human_remodelled-5").gameObject.transform.Find ("shadow_human_rigged_001_geo").gameObject;
				//GameObject m_RiggedAgent = NewAgent.transform.Find ("shadow_human_rigged_001_geo").gameObject;
				Renderer m_RiggedAgentRenderer = m_RiggedAgent.GetComponent<SkinnedMeshRenderer> ();
				m_RiggedAgentRenderer.material = m_MarkerRenderer.material; 
			}
			i += 1;
			yield return null;
		}
		Debug.Log ("==Agent Spawn Complete==");
		AgentSpawnComplete = true;
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
