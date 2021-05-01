using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetConnection : MonoBehaviour{

	private HingeJoint Hinge;
	private Vector3 AnchorPos;

	// Start is called before the first frame update
	void Start(){
		// get hinge
		Hinge = this.GetComponent<HingeJoint> ();
		// don't auto connect anchor  point
		Hinge.autoConfigureConnectedAnchor = false;
		// determine the anchor point
		AnchorPos = Hinge.anchor;
	}

    // Update is called once per frame
    void Update(){
		Hinge.anchor = AnchorPos;
	}
}
