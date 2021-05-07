using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientCube : MonoBehaviour
{
    
    public bool NearCube;
    private GameObject cube;
    private Rigidbody CubeRbody;
	private AI ai;

    // Start is called before the first frame update
    void Start()
    {
        NearCube = false;
		ai = GetComponent<AI> ();
		Debug.Log ("bay" + ai.bay.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (NearCube)
        {
            OrientCubeTowardAgent();
        }
    }

    // agent hits cube
    void OnCollisionEnter(Collision col){
        // Hit Cube.
        if (col.gameObject.CompareTag("cube")) {
            cube = col.gameObject;
            Debug.Log("Hit " + cube.name);
            NearCube = true;
            CubeRbody = cube.GetComponent<Rigidbody>();
		}
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger " + other.tag);
        if (other.tag == "bay")
        {
            // release cube
            NearCube = false;
			cube.transform.SetParent (null);
		}
    }

    // push cube
    void OrientCubeTowardAgent(){
        // If Agent is defined and close to this cube
        if (cube != null && Vector3.Distance(cube.transform.position, transform.position) <= 1.5f) {
            // face direction of travel
            // NOT WORKING
            if (CubeRbody.velocity != Vector3.zero) {
                // Set constraints to keep hovering
                CubeRbody.angularDrag = 2.0f;
                // rotate to face bay (NOTE: check this works if bay changes!)
                float speed = 0.5f;
                var qTo = Quaternion.LookRotation(ai.bay.position - cube.transform.position);
                qTo = Quaternion.Slerp(cube.transform.rotation, qTo, speed * Time.deltaTime);
                CubeRbody.MoveRotation(qTo);

				// position in front of the agent
				//CubeRbody.MovePosition(this.transform.position + transform.forward);

				// cube parented by agent
				// probably make kinematic
				cube.transform.SetParent (this.transform);
			}
        }
        // agent is no longer close to cube - reset everything
        else if (cube != null && Vector3.Distance(cube.transform.position, transform.position) >= 1.5f)  {
            NearCube = false;
            CubeRbody.angularDrag = 0.05f;
        }
    }
}
