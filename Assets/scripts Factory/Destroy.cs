using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // destroy cubes
    public void DestroyCubes()
    {
        // get all cubes
        GameObject[] cubes = GameObject.FindGameObjectsWithTag ("cube");
        // destroy all cubes
        foreach (GameObject cube in cubes) {
            if (cube.name != "CubeMaster")
            {
                Destroy(cube);
            }
            
        }
    }

    // destroy agents
    public void DestroyAgents()
    {
        // get all agents
        GameObject[] agents = GameObject.FindGameObjectsWithTag ("agent");
        // destroy all agents
        foreach (GameObject agent in agents) {
            if (agent.name != "Agent0")
            {
                Destroy(agent);
            }
        }
    }
}
