using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubes : MonoBehaviour
{
    public GameObject SpawnedCube;
	private float SpawnWait;
	public int NumCubes;
	public float SpawnPosMaxX;
	public float SpawnPosMinX;
	public float SpawnPosMaxZ;
	public float SpawnPosMinZ;
	public bool SpawnComplete;


	// Start is called before the first frame update
	void Start() {
		SpawnWait = 0.01F;
		StartCoroutine (Spawner ());
		SpawnComplete = false;
    }

    // Update is called once per frame
    void Update() {
        
    }

	
	IEnumerator Spawner (){
		//float StartPosX = this.transform.position.x + Random.Range(SpawnPosMinX, SpawnPosMaxX);
		//float StartPosZ = this.transform.position.x + Random.Range(SpawnPosMinZ, SpawnPosMaxZ);
		//float StartPosX = this.transform.position.x + SpawnPosMinX;
		//float StartPosZ = this.transform.position.z + SpawnPosMinZ;
		float StartPosX = this.transform.position.x;
		float StartPosZ = this.transform.position.z;
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
			NewCube.transform.parent = transform;
			yield return new WaitForSeconds (SpawnWait);
		}
		//Debug.Log("==Spawn Complete==");
		SpawnComplete = true;
	}
}
