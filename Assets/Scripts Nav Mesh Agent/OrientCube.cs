using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientCube : MonoBehaviour
{
    
    public bool NearCube;
    private GameObject cube;
    private Rigidbody CubeRbody;

    // Start is called before the first frame update
    void Start()
    {
        NearCube = false;
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
            Debug.Log("hit- " + cube.name);
            NearCube = true;
            CubeRbody = cube.GetComponent<Rigidbody>();
            // position in front of the agent
            Debug.Log("agent:" + this.transform.position);
            // rotate (might be better rotated to face bay)
            float speed = 5.0f;
            var qTo = Quaternion.LookRotation(this.transform.position - cube.transform.position);
            qTo = Quaternion.Slerp(cube.transform.rotation, qTo, speed * Time.deltaTime);
            CubeRbody.MoveRotation(qTo);
            //cube.transform.rotation = Quaternion.LookRotation(CubeRbody.velocity, Vector3.up);
        }
    }

    // push cube
    void OrientCubeTowardAgent(){
        // If Agent is defined and close to this cube
        if (cube != null && Vector3.Distance(cube.transform.position, transform.position) <= 1.3f) {
            // face direction of travel
            // NOT WORKING
            if (CubeRbody.velocity != Vector3.zero) {
                // Set constraints to keep hovering
                CubeRbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                CubeRbody.angularDrag = 1;
                //cube.transform.rotation = Quaternion.LookRotation(CubeRbody.velocity, Vector3.up);
            }
        }
        // agent is no longer close to cube - reset everything
        else if (cube != null && Vector3.Distance(cube.transform.position, transform.position) >= 1.3f)  {
            NearCube = false;
            CubeRbody.angularDrag = 0.05f;
            // un freeze constraints
            //CubeRbody.constraints = RigidbodyConstraints.None;
        }
    }
}
