using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    
    // this class reads the json file in streaming assets and set the behaviour coefficients for each agent.
    // this script needs to be attached to the agents
    // this reloads the json file every spawn

    // behaviour coefficients 
    // steal
    // 0.0 - take cubes from another bay 0% of the time
    // 0.5 - take cubes from another bay 50% of the time
    // 1.0 - take cubes from another bay 100% of the time // harm
    public float Dishonesty;
    
    // RobinHood - only steal from wealthy bays with 4 or more resources
    // only works when Steal > 0
    public bool RobinHood;

    // help other agents by choosing another bay to push too
    // 0.0 - never help other agents 
    // 0.5 - help other agents 50% of the time whenever have 3 cubes
    // 1.0 - always help other agents whenever have 3 cubes
    public float Charity;

    // compete for resources - leave a cube if other agents are trying to get it
    // 0.0 compete - leave cubes if another agent is near cube / pushing cube
    // 0.5 compete - ignore if another agent is near cube 
    // 1.0 compete - target cubes if another agent is nearby / pushing a cube
    public float Competitive;

    // stockpile / abstain
    // 0.0 - don't collect anymore cubes after stockpiled 3
    // 0.1 - don't collect anymore cubes after stockpiled 4
    // 0.9 - don't collect anymore cubes after stockpiled 12
    // 1.0 - collect as many cubes as possible
    public float Greed;

	public string path;
	public string jsonString;



	// Start is called before the first frame update
	void Start()
    {
        // default test values
		Dishonesty = 0.0f;
        RobinHood = false;
        Charity = 1.0f;
        Competitive = 0.0f;
        Greed = 0.0f;

        // get the json data from Agents.json
		path = Application.streamingAssetsPath + "/Agents.json";
		jsonString = File.ReadAllText (path);
        // use custom class below to access json data
		Agents jsonData = JsonUtility.FromJson<Agents> (jsonString);
		// get the behaviour coefficients from the json list
        SetAgentBehaviours(jsonData.agent);		
    }


    void SetAgentBehaviours(List<AgentObjects> agentsList)
    {
        // loop through the json list
        foreach(AgentObjects agent in agentsList)
        {
            // if agent.name == this gameobject name // set vars / behaviour coefficients
            if (agent.name == gameObject.name)
            {
                Debug.Log (agent.name + " dishonesty:"+agent.dishonesty+ " charity:"+agent.charity);
                Dishonesty = agent.dishonesty;
                RobinHood = agent.robinHood;
                Charity = agent.charity;
                Competitive = agent.competitiveness;
                Greed = agent.greed;
            }
            
        }
    }
}

// custom class to access json data
[System.Serializable]
public class Agents 
{
	public string uktime;
	public string time;
    public List<AgentObjects> agent;
}

// custom class to access array in json data
[System.Serializable]
public class AgentObjects 
{
	public string name;
	public float dishonesty;
    public bool robinHood;
    public float charity;
    public float competitiveness;
    public float greed;
}
