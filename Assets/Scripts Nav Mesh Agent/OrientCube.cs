using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientCube : MonoBehaviour
{
    
    public bool NearCube;
    private GameObject cube;
    private Rigidbody CubeRbody;
	private AI ai;
    Animator anim;


	// Start is called before the first frame update
	void Start()
    {
        NearCube = false;
		ai = GetComponent<AI> ();
        anim = this.GetComponent<Animator> ();
		//Debug.Log ("bay" + ai.bay.position);
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
            //anim.SetBool("IsPush", true);
            cube = col.gameObject;
            NearCube = true;
            CubeRbody = cube.GetComponent<Rigidbody>();
		}
    }

	// enter bay
    void OnTriggerEnter(Collider other) {
        //Debug.Log("Trigger " + other.tag);
        if (other.tag == "bay")
        {
            // release cube
            NearCube = false;
			//CubeRbody.constraints = RigidbodyConstraints.None;
		}
    }

    // push cube
    void OrientCubeTowardAgent(){
        // If Agent is defined and close to this cube
        if (cube != null && Vector3.Distance(cube.transform.position, transform.position) <= 1.2f) {
            // face direction of travel
            // NOT WORKING
            if (CubeRbody.velocity != Vector3.zero) {
                anim.SetBool("IsPush", true);
                // Set constraints to keep hovering
                CubeRbody.angularDrag = 2.0f;
                // rotate to face bay (NOTE: check this works if bay changes!)
                var qTo = Quaternion.LookRotation(ai.bay.transform.position - cube.transform.position);
                CubeRbody.MoveRotation(qTo);

				// Set constraints 
				CubeRbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

				//
				cube.transform.SetParent (this.transform.parent);
				// position in front of the agent
				//CubeRbody.MovePosition(this.transform.position + transform.forward);
				// use force
				//CubeRbody.AddForce (((this.transform.position + transform.forward) - cube.transform.position) * 1000.5f);
				//CubeRbody.AddForce ((ai.transform.position - cube.transform.position) * 1000.5f);
				//CubeRbody.AddForce (((this.transform.position + ((ai.bay.transform.position - this.transform.position).normalized * 1)) - cube.transform.position) * 35.0f);
			}
		}
        // agent is no longer close to cube - reset everything
        else if (cube != null && Vector3.Distance(cube.transform.position, transform.position) >= 1.5f)  {
            NearCube = false;
			CubeRbody.constraints = RigidbodyConstraints.None;
			cube.transform.SetParent (null);
            anim.SetBool("IsPush", false);

		}
    }
}
