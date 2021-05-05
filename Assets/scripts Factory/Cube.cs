using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    public bool Moving;
    public Vector3 StartPos;
    public Vector3 LastPos;
    public bool IsFollowingMagnet;
    public bool Collision;
    private Vector3 FollowOffset;
    private Rigidbody CubeRigidBody;
    public GameObject CraneMagnet;   

    public float speed;

	public Vector3 DirectionToMagnet;
	public Vector3 movement;

    // Start is called before the first frame update
    void Start() {
        CubeRigidBody = GetComponent<Rigidbody>();
        CraneMagnet = GameObject.Find("CraneMagnet");
        Moving = false;
        LastPos = transform.position;
		StartPos = CreateStartPos(transform.position);
        //Collision = false;
        IsFollowingMagnet = false;
        Collision = false;
    }

    // Update is called once per frame 
    void Update() {
        // check if cube is moving
        IsMoving();

        // set target at bottom of box collider
		float Bottom = 0.8f;
		// get direction
		Vector3 Target = new Vector3 (CraneMagnet.transform.position.x, CraneMagnet.transform.position.y-Bottom, CraneMagnet.transform.position.z);
		DirectionToMagnet = (Target - this.transform.position).normalized;
    }

    void IsMoving(){
        Vector3 curPos = this.transform.position;
     	if(curPos == LastPos) {
        	Moving = false;
     	} else {
			 Moving = true;
		 }
     	LastPos = curPos;
    }


    void FixedUpdate (){
		if (IsFollowingMagnet) {
			// follow the magnet - triggered by MoveGantry
			FollowMagnet (DirectionToMagnet);
		}
	}

    void FollowMagnet (Vector3 direction){
		// disable gravity
		CubeRigidBody.useGravity = false;
		//Freeze all rotations
		CubeRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		//CubeRigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;
		//CubeRigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
		//speed = 0.7f;
		// make speed related to distance
		float distance = Vector3.Distance (CraneMagnet.transform.position, this.transform.position);
		// set speed related to distance 
		speed = distance * 3.1f; //(float)Math.Pow (distance, 4.0f);
		if (distance > 1.3f) {
			speed = 20.0f;
		}
		//Debug.Log ("distance=" + distance);
		Debug.Log ("speed=" + speed);
		// move cube
		CubeRigidBody.MovePosition (transform.position + (direction * speed * Time.deltaTime));
	}

	// COLLISION USED TO DROP OFF CUBE ON CONTACT WITH GROUND (OR OTHER CUBE)
	// When the cube collides 
	private void OnCollisionEnter (Collision collision) {
		//Debug.Log ("COLLISION - " + name + " collides with " + collision.gameObject.name);
		// if cube hits something other than magnet 
		// cube stops following magnet
		if (collision.gameObject.tag != "cranemagnet") {
			//Debug.Log(name + " stop following / use gravity");
			IsFollowingMagnet = false;
			Collision = true;
			// turn on physics    
			CubeRigidBody.useGravity = true;
			//unfreeze rotation
			//CubeRigidBody.constraints = RigidbodyConstraints.None;
			//stop CraneMagnet following
		} 
	}

	// create a start position that is 3 cubes width nearer the centre of the yellow box
	Vector3 CreateStartPos(Vector3 loc){
		if(loc.x > 0) {
			loc.x -= 3;
		} else {
			loc.x += 3;
		}
		if(loc.z > 0) {
			loc.z -= 3;
		} else {
			loc.z += 3;
		}
		return loc;
	}
}
