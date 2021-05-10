using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InBay : MonoBehaviour
{

	public NavMeshObstacle navMeshObstacle;


	// Start is called before the first frame update
	void Start()
    {
		navMeshObstacle = GetComponent<NavMeshObstacle> ();

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	// enter bay
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "bay") {
			// enable NaveMeshObstacle in state.cs
		}
	}

	// exit bay
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "bay") {
			// disable NaveMeshObstacle
			navMeshObstacle.enabled = false;
		}
	}
}
