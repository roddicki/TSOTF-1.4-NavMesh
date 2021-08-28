using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGantry : MonoBehaviour {

    private Vector3 gantryPos;
    private float speed;
    private float TargetCubeX;
    private float TargetCubeXOrigin;
    private float TargetCubeY;
    private float TargetCubeYOrigin;
    private float TargetCubeZ;
    private float TargetCubeZOrigin;
    private GameObject[] Cubes;
    private GameObject TargetCube;
    private Cube TargetCubeCS;
    private MoveCrane Crane;
    public bool GantryBusy;
    private bool GantryArrived;
    private CraneMagnet CraneMagnet;
	private GameObject craneMagnet;
    private WinchCable Cable;
    private Spawn Spawn;
    private CubeManager CubeManager;


    void Start() {
        // get all cubes
        Cubes = GameObject.FindGameObjectsWithTag("cube");
        // set speed
        speed = 6.5f;
        // GantryBusy = false;
        GantryArrived = false;
        // initialise with self position
        TargetCubeX = transform.position.x; 
        // get the Crane <MoveCrane>    
        Crane = GameObject.Find("Crane").GetComponent<MoveCrane>();
		// get the Crane Magnet
		craneMagnet = GameObject.Find ("CraneMagnet");
		CraneMagnet = craneMagnet.GetComponent<CraneMagnet>();
		 
		Cable = GameObject.Find("Cable").GetComponent<WinchCable>();
        Spawn = GameObject.Find("GameManager").GetComponent<Spawn>();
        CubeManager = GameObject.Find("GameManager").GetComponent<CubeManager>();
    }


    void FixedUpdate() {
        // so BeginCraneOperations() runs continously
        //Debug.Log("UPDATE TargetCube: " + TargetCube + "GantryBusy: "+ GantryBusy); 

        if(TargetCube != null && GantryBusy == false && Spawn.CubeSpawnComplete) {
            StartCoroutine(BeginCraneOperations());
            GantryBusy = true;
        }

        // check if any cubes moved
        CheckCubes();
        // get the location of the next Cube that has moved
        getTargetCubeCoords();  
        
          
    }

    // COROUTINES
    IEnumerator BeginCraneOperations(){
        Debug.Log("CRANE BEGIN CraneOperations");
        Debug.Log("CRANE TargetCube="+TargetCube);
        // 1. move Gantry and Crane to cube
        yield return StartCoroutine(MoveToCube());
        // 2. Lower winch / magnet
        yield return StartCoroutine(LowerWinchPickUp());
        // Pick up Cube
        yield return StartCoroutine(CheckPickedUpCube());
        // 4. Raise winch / magnet
        yield return StartCoroutine(RaiseWinchMagnet());
        // check cube is following
        //if(CubeIsFollowing()){
        // Move Gantry and Crane to cube start position
        yield return StartCoroutine(SetGantryCraneArrived());
        // 5. GantryArrived = false;
        yield return StartCoroutine(MoveToCubeStartPos());
        // 6. Lower winch / magnet
        yield return StartCoroutine(LowerWinchDropOff());
        // 7. Raise winch / magnet
        yield return StartCoroutine(RaiseWinchAfterDropOff());
        //};
        // move cube to end of list
        yield return StartCoroutine(CubeToListEnd());
        // 8. reset
        yield return StartCoroutine(Reset());
        Debug.Log("END CRANE CraneOperations");
        Debug.Log("...");
        yield break;
    }

    // Move Gantry and Crane to cube
    IEnumerator MoveToCube() {
        Debug.Log("1. CRANE move to cube new pos");
        // move the Gantry
        while (GantryArrived == false || Crane.CraneArrived == false) {
            MoveToNewPos(TargetCubeX);
            Crane.MoveToNewPos(TargetCubeZ);
            yield return null;
        }
    }

    // Lower winch / magnet
    IEnumerator LowerWinchPickUp() {
        Debug.Log("2. CRANE Lower winch, pick up");
        CraneMagnet.Collision = false;
        // Lower cable and magnet until collides with a cube or ground
        while (CraneMagnet.Collision == false) {
            Cable.LowerCable();
            yield return null;
        } 
    }

    // check target cube is set to the cube being carried
    IEnumerator CheckPickedUpCube(){
        Debug.Log("3. CRANE Check targetcube = " + TargetCube);
        // change target cube to the child of the CraneMagnet
        foreach(Transform child in craneMagnet.transform)
        {
            //Debug.Log("3. CRANE carrying " + child);
            if(child.tag == "cube") {
                TargetCube = child.gameObject;
            }
        }
        Debug.Log("3. CRANE new targetcube = " + TargetCube);
        yield break;
    }

    // Raise winch / magnet
    IEnumerator RaiseWinchMagnet() {
        Debug.Log("4. CRANE Raise winch");
        Cable.RaiseComplete = false;
        // Raise Cable and Magnet
        while (Cable.RaiseComplete == false) {
            Cable.RaiseCable();
            yield return null;
        } 
    }

    IEnumerator SetGantryCraneArrived(){
        GantryArrived = false;
        Crane.CraneArrived = false;
        yield break;
    }

    // Move to Cube start point
    IEnumerator MoveToCubeStartPos() {
        // reset Lower and Raise cable flags
        CraneMagnet.Collision = false;
        Cable.RaiseComplete = false;
        Debug.Log("5. CRANE move to TargetCube start pos");
        // Move to Cube start point
        while (GantryArrived == false || Crane.CraneArrived == false) {
            // move Gantry
            MoveToNewPos(TargetCubeXOrigin);
            // move Crane
            Crane.MoveToNewPos(TargetCubeZOrigin);
            yield return null;
        }
    } 

    // NOT FECKIN WORKING
    // Lower winch / magnet
    IEnumerator LowerWinchDropOff() {
        //Debug.Log("6. CRANE DROP CUBES - Lower winch ");
		// stop all cubes following
		// create list of crane magnet children
        List<GameObject> cubeChildren = new List<GameObject>();

		for (int i = 0; i < craneMagnet.transform.childCount; i++) {
			Transform child = craneMagnet.transform.GetChild (i);
            cubeChildren.Add(child.gameObject);
		}
        // go through list and un parent each cube
        foreach (GameObject cube in cubeChildren)
        {
            if (cube.tag == "cube")
            {
                Debug.Log ("6. CRANE Drop off " + cube.gameObject.name);
                // un parent
				cube.gameObject.transform.parent = null;
				Rigidbody rb = cube.gameObject.GetComponent<Rigidbody> ();
				// use gravity
				rb.useGravity = true;
				// non kinematic
				rb.isKinematic = false;
                //unfreeze any rotation
                rb.constraints = RigidbodyConstraints.None;
				// set not following bool
                yield return new WaitForSeconds(0.1f);
				cube.gameObject.GetComponent<Cube> ().IsFollowingMagnet = false;
            }
        }
        //Debug.Log("CRANE LowerWinchDropOff complete");
        yield return null;
    }


    // Raise winch / magnet
    IEnumerator RaiseWinchAfterDropOff() {
        Debug.Log("7. CRANE Raise winch");
        Cable.RaiseComplete = false;
        // Raise Cable and Magnet
        while (Cable.RaiseComplete == false) {
            Cable.RaiseCable();
            yield return null;
        } 
    }

    // move cube to end of CubesOutside list (CubeManager script) so that the crane does not attempt to pick it up two times in a row
    IEnumerator CubeToListEnd(){
        //Debug.Log("CRANE TargetCube = " + TargetCube);
        if (TargetCube != null) {
            CubeManager.CubeToListEnd(TargetCube);
        }
        yield break;
    }

    // reset 
    IEnumerator Reset() {
        Debug.Log("8. CRANE Reset");
        // reset cube ready for next cube that is moved
        if (TargetCube != null) {
            // reset TargetCube 
            TargetCube = null;
        }
        //ResetCube();
        //Debug.Log("CRANE Reset TargetCube="+TargetCube);
        GantryArrived = false;
        Crane.CraneArrived = false;
        // operations complete release gantry for further tasks
        GantryBusy = false;
        yield break;
    }



    // FUNCTIONS
    // check if any cube is outside the yellow box
    void CheckCubes(){
        // get cube at top of CubesOutside list
        List<string> CubesOut = GameObject.Find("GameManager").GetComponent<CubeManager>().CubesOutside;
        // if CubesOutside is not empty
        if(CubesOut.Count > 0) {
            // get cube at top of CubesOutside list
            TargetCube = GameObject.Find(CubesOut[0]);
            if (TargetCube != null) {
                TargetCubeCS = GameObject.Find(CubesOut[0]).GetComponent<Cube>();
            } 
        }
    }

    // get first cube that moved from GameManager
    void getTargetCubeCoords(){
        // new target
        if (TargetCube != null) {
            TargetCubeX = TargetCube.transform.position.x;
            TargetCubeY = TargetCube.transform.position.y;
            TargetCubeZ = TargetCube.transform.position.z;
            TargetCubeXOrigin = TargetCube.GetComponent<Cube>().StartPos.x;
            TargetCubeYOrigin = TargetCube.GetComponent<Cube>().StartPos.y;
            TargetCubeZOrigin = TargetCube.GetComponent<Cube>().StartPos.z;
            //Debug.Log("TargetCube " + TargetCube.name);
        }
    }

    bool CubeIsFollowing(){
        // if cube not following reset
        if(TargetCubeCS.Moving == false) {
            return false;
        } else {
            return true;
        }
    }

    // check if Gantry arrived at a point X
    bool ArrivedAt(float X) {
        float SelfPosZ = transform.position.z;
        float SelfPosY = transform.position.y;
        Vector3 targetX = new Vector3(X, SelfPosY, SelfPosZ);
        // if reached target x pos        
        if(Vector3.Distance(transform.position, targetX) < 0.2f){
            return true;
        } else {
            return false;
        }
    }

    // if crane magnet comes near other cubes
    bool NearGround(){
        Collider[] hitColliders = Physics.OverlapSphere(CraneMagnet.transform.position, 1.0f);
        //Debug.Log("hitColliders.Length" + hitColliders.Length);
        int  i = 0;
        foreach (Collider hit in hitColliders) {
           if(hit.tag == "ground"){
               i++;
           } 
        }
        if(i > 0) {
            Debug.Log("CRANE ground close by - drop off");
            return true;
        } else {
            return false;
        }
    }


    // if crane magnet comes near other cubes
    bool NearCubes(){
        Collider[] hitColliders = Physics.OverlapSphere(CraneMagnet.transform.position, 2.5f);
        //Debug.Log("hitColliders.Length" + hitColliders.Length);
        int  i = 0;
        foreach (Collider hit in hitColliders) {
           if(hit.tag == "cube"){
               i++;
           }  
        }
        if(i > 2) {
            Debug.Log("CRANE Cubes close by - drop off");
            return true;
        } else {
            return false;
        }
        // if (hitColliders.Length > 5){
        //     Debug.Log("Found something!");
        //     return true;
        // } else {
        //     return false;
        // }
    }

    // move Gantry to new coordinates
    void MoveToNewPos(float X) {
        gantryPos = transform.position;
        float step = speed * Time.deltaTime;
        float SelfPosZ = transform.position.z;
        float SelfPosY = transform.position.y;
        // set the new position. 
        Vector3 targetX = new Vector3(X, SelfPosY, SelfPosZ);   // only move to x position. 

        // if reached target x pos        
        if(Vector3.Distance(transform.position, targetX) < 0.2f){
            //It is within ~0.1f range, do stuff
            //Debug.Log("Gantry arrived");
            // stand in variable // need to have picked up the cube here
            GantryArrived = true;
            // check that the gantry is back at the cube origin point
            // do this in a new function
            //step = 0;
        } 
        
        transform.position = Vector3.MoveTowards (transform.position, targetX, step);
        //Debug.Log( Vector3.Distance(transform.position, targetX) );
    }
}
