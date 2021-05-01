using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CraneMagnet : MonoBehaviour {
    
    public bool Collision;
    

    //When the magnet collides with the cube
    private void OnCollisionEnter(Collision collision) {
        Debug.Log("BANG - " + name + " collides with " + collision.gameObject.name);
        Collision = true;
    }

}
