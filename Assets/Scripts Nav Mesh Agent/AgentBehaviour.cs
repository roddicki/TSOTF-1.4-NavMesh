using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    // behaviour coefficients 

    // steal
    // 0.0 - take cubes from another bay 0% of the time
    // 0.5 - take cubes from another bay 50% of the time
    // 1.0 - take cubes from another bay 100% of the time // harm
    public float Steal;
    
    // RobinHood - only steal from wealthy bays with 4 or more resources
    // only works when Steal > 0
    public bool RobinHood;

    // help other agents by choosing another bay to push too
    // 0.0 - never help other agents 
    // 0.5 - help other agents 50% of the time whenever have 3 cubes
    // 1.0 - always help other agents whenever have 3 cubes
    public float Help;

    // compete for resources - leave a cube if other agents are trying to get it
    // 0.0 compete - leave cubes if another agent is near cube / pushing cube
    // 0.5 compete - ignore if another agent is near cube 
    // 1.0 compete - target cubes if another agent is nearby / pushing a cube
    public float Compete;

    // stockpile / abstain
    // 0.0 - don't collect anymore cubes after stockpiled 3
    // 0.1 - don't collect anymore cubes after stockpiled 4
    // 0.9 - don't collect anymore cubes after stockpiled 12
    // 1.0 - collect as many cubes as possible
    public float Greed;



    // Start is called before the first frame update
    void Start()
    {
        // test values
        Steal = 1.0f;
        RobinHood = false;
        Help = 0.0f;
        Compete = 0.5f;
        Greed = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
