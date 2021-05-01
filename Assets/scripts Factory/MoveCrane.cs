using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCrane : MonoBehaviour {

    public float speed;
    public bool CraneArrived;


    void Start() {
        speed = 6.5f;
        CraneArrived = false;
    }


    void Update() {

    }

    //FUNCTIONS

    // test
    public void Test(string msg) {
        Debug.Log(msg);
    }

    // move to new coordinates
    public void MoveToNewPos(float CubeZ) {
        Vector3 cranePos = transform.position;
        float step = speed * Time.deltaTime;
        float SelfPosY = transform.position.y;
        float SelfPosX = transform.position.x;
        Vector3 targetZ = new Vector3(SelfPosX, SelfPosY, CubeZ+0.2f); //add 0.2 for crane alignmenet / adjustment

        transform.position = Vector3.MoveTowards (transform.position, targetZ, step);
        
        // if reached target x pos        
        if(Vector3.Distance(transform.position, targetZ) < 0.2f){
            //It is within range, do stuff
            Debug.Log("crane arrived");
            CraneArrived = true;
        } 
    }
}
