using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchCable : MonoBehaviour {
    // Start is called before the first frame update
    private Vector3 CableStartPos;
    private Vector3 ScaleChange;
    public bool RaiseComplete;
    public bool LowerComplete;
    private HingeJoint Hinge;
	private Vector3 AnchorPos;
    

    void Start() {
        // get hinge
		Hinge = this.GetComponent<HingeJoint> ();
		// don't auto connect anchor  point
		Hinge.autoConfigureConnectedAnchor = false;
		// determine the anchor point
		AnchorPos = Hinge.anchor;//new Vector3 (0.0f,1.0f, 0.0f);
        // set scale change increment
        ScaleChange = new Vector3(0, 0.05f, 0);
        //PositionChange = new Vector3(0, -0.03f, 0);
        CableStartPos = transform.position;
        RaiseComplete = false;
    }

    // Update is called once per frame
    void Update() {
        //MoveCable();   
    }

    public void LowerCable(){
        //Debug.Log("LowerCable");
        this.transform.localScale += ScaleChange;
		Hinge.anchor = AnchorPos;
    }

    public void RaiseCable(){
        // move HingeJoint
        // scale and reposition Cable
        if (transform.position.y < CableStartPos.y) {
            this.transform.localScale -= ScaleChange;
		    Hinge.anchor = AnchorPos;
            RaiseComplete = false;
        } else {
            RaiseComplete = true;
        }
        
    }
}
