using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePosition : MonoBehaviour {
    public int Outside;
    private GameObject YellowBox;
    private GameObject GridBox;
    private Bounds YellowBoxBounds;
    private Bounds GridBoxBounds;

    // Start is called before the first frame update
    void Start(){
        Outside = 0;
        YellowBox = GameObject.Find("Ground Decal Square");
        YellowBoxBounds = YellowBox.GetComponent<Collider>().bounds;
        GridBox = GameObject.Find("Grid");
        GridBoxBounds = GridBox.GetComponent<Collider>().bounds;
    }

    // Update is called once per frame
    void Update(){
        InYellowBox();
    }


    // check if cube outside or inside yellow box collider
	void InYellowBox(){
        // if cube is contained by yellow box collider
        if(YellowBoxBounds.Contains(this.transform.position)) {
            Outside = 0; //false
        } 
        // if cube is contained by grid box collider
        else if(GridBoxBounds.Contains(this.transform.position)) {
            Outside = 1; // true
        }
        // else must be out of the game
        else {
            Outside = 2; //out of the game
        }
	}

}
