using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{

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
        // get bay 0
        GameObject Bay0 = GameObject.Find("Agent0-bay");
        // get all agents
        GameObject[] agents = GameObject.FindGameObjectsWithTag ("agent");
        // destroy all agents
        foreach (GameObject agent in agents) {
            Destroy(agent);
        }
    }

    // destroy ragdolls
    public void DestroyRagdolls() {
        GameObject[] ragdolls = GameObject.FindGameObjectsWithTag ("ragdoll");
        // destroy all ragdolls
        foreach (GameObject ragdoll in ragdolls) {
            Destroy(ragdoll);
        }
    }
}
