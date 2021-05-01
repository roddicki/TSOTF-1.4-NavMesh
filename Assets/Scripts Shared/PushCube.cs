using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCube : MonoBehaviour
{
    // character animation
    AnimateMe AnimationController;
    public bool NearCube;
    private GameObject cube;
    private Rigidbody CubeRbody;

    // Start is called before the first frame update
    void Start()
    {
        AnimationController = GetComponent<AnimateMe>();
        NearCube = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (NearCube)
        {
            OrientCube();
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
        }
    }

    // push cube
    void OrientCube(){
        // If Agent is defined and close to this cube
        if (cube != null && Vector3.Distance(cube.transform.position, transform.position) <= 1.3f) {
            AnimationController.IsPushing = true;
            // face direction of travel
            // NOT WORKING
            if (CubeRbody.velocity != Vector3.zero) {
                // Set constraints to keep hovering
                CubeRbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                CubeRbody.angularDrag = 1;
                cube.transform.rotation = Quaternion.LookRotation(CubeRbody.velocity, Vector3.up);
            }
        }
        // agent is no longer close to cube - reset everything
        else if (cube != null && Vector3.Distance(cube.transform.position, transform.position) >= 1.3f)  {
            NearCube = false;
            CubeRbody.angularDrag = 0.05f;
            // un freeze constraints
            //CubeRbody.constraints = RigidbodyConstraints.None;
            AnimationController.IsPushing = false;
        }
    }
}
