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

	public Vector3 followingOffset;

    // Start is called before the first frame update
    void Start() {
        CubeRigidBody = GetComponent<Rigidbody>();
        CraneMagnet = GameObject.Find("CraneMagnet");
        Moving = false;
        LastPos = transform.position;
		StartPos = CreateStartPos(transform.position);
        //Collision = false;
		followingOffset = new Vector3(0f,-1.58f,0f);
        IsFollowingMagnet = false;
        Collision = false;
    }

    // Update is called once per frame 
    void Update() {
        // check if cube is moving
        IsMoving();
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

	// follow magnet
	public void ParentToMagnet()
	{
		// If CraneMagnet has 1 child parent to magnet
		if (CraneMagnet.transform.childCount < 2) {
			// if magnet doesn't already have a cube following
			Debug.Log ("CRANE parent " + this.name + " to magnet");
			// disable gravity and make kinematic
			CubeRigidBody.useGravity = false;
			CubeRigidBody.isKinematic = true;
			// parent
			this.transform.parent = CraneMagnet.transform;
			// move to magnet
			this.transform.position = CraneMagnet.transform.position + followingOffset;
			IsFollowingMagnet = true;
		}
	}

	// COLLISION USED TO DROP OFF CUBE ON CONTACT WITH GROUND (OR OTHER CUBE)
	// When the cube collides 
	private void OnCollisionEnter (Collision collision) {
		//Debug.Log ("COLLISION - " + name + " collides with " + collision.gameObject.name);
		// if cube hits magnet 
		if (collision.gameObject.tag == "cranemagnet" && IsFollowingMagnet == false) {
			ParentToMagnet();
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
