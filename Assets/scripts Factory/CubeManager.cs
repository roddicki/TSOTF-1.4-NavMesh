using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour {
    public List<string> CubesOutside;
    public List<string> CubesInside;
    private GameObject[] Cubes;

    // Start is called before the first frame update
    void Start(){
        // get all cubes
        Cubes = GameObject.FindGameObjectsWithTag("cube");
    }

    // Update is called once per frame
    void Update(){
        GetAllCubes();
        CheckCubesOutside();
    }

    // get all cubes
    void GetAllCubes(){
        // get all cubes
        Cubes = GameObject.FindGameObjectsWithTag("cube");
    }

    // check if cube is outside yellow box
    void CheckCubesOutside(){
        // check each cube
        for (int i = 0; i < Cubes.Length; i++) { 
            //  if cube outside game
            if(Cubes[i].GetComponent<CubePosition>().Outside == 2) {
                // remove from all lists
                if(CubesOutside.Contains(Cubes[i].name)){
                    CubesOutside.Remove(Cubes[i].name);
                }
                if(CubesInside.Contains(Cubes[i].name)){
                    CubesInside.Remove(Cubes[i].name);
                }
            }
            // if cube outside and not in CubesOutside list
            else if(Cubes[i].GetComponent<CubePosition>().Outside == 1 && CubesOutside.Contains(Cubes[i].name) == false){
                // add to list - will order list by oldest first
                CubesOutside.Add(Cubes[i].name);
                // if in CubesInside list remove it
                if(CubesInside.Contains(Cubes[i].name)){
                    CubesInside.Remove(Cubes[i].name);
                }
            } 
            // else if cube inside and not in CubesInside list
            else if(Cubes[i].GetComponent<CubePosition>().Outside == 0 && CubesInside.Contains(Cubes[i].name) == false){
                // add to list
                CubesInside.Add(Cubes[i].name);
                // if in CubesOutside list remove it
                if(CubesOutside.Contains(Cubes[i].name)){
                    CubesOutside.Remove(Cubes[i].name);
                }
            }  
            
        }
    }

    // move a cube to the end of the CubesOutside list
    public void CubeToListEnd(GameObject Cube){
        if(CubesOutside.Contains(Cube.name)){
            CubesOutside.Remove(Cube.name);
            CubesOutside.Add(Cube.name);
        }
    }
    
}
