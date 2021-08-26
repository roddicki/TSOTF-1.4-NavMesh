using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CraneMagnet : MonoBehaviour {
    
    public bool Collision;
    

    //When the magnet collides with the anything 
    private void OnCollisionEnter(Collision collision) {
        Debug.Log("BANG - " + name + " collides with " + collision.gameObject.name);
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "truss" || collision.gameObject.tag == "cube")
        {
            Collision = true;
        }        
    }

}
