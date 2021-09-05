using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGroundMaterial : MonoBehaviour
{
    public GameObject ground;
	public GameObject marker;
	Material m_GroundMaterial; //cached on Awake()
	Material m_MarkerMaterial; //cached on Awake()
	public Material goalScoredMaterial;
	public Material goalScoredMarkerMaterial;
    Renderer m_GroundRenderer;
	Renderer m_MarkerRenderer;
	GoalDetect GoalDetect;
	// dead or alive
	private AI aI;

	// Start is called before the first frame update
	void Start()
    {
        // Get the ground renderer so we can change the material when a goal is scored
        m_GroundRenderer = ground.GetComponent<Renderer>();
		m_MarkerRenderer = marker.GetComponent<Renderer> ();
		// Starting material
		m_GroundMaterial = m_GroundRenderer.material;
		m_MarkerMaterial = m_MarkerRenderer.material;
		// get goal detect script
		GoalDetect = GetComponent<GoalDetect>();
	}

	void OnTriggerEnter(Collider other) 
    {
		// assign aI to the agent AI component to get its current state (and if Death) 
		// assign here not in Start to be sure agent is instantiated
		if (aI == null) {
			// find agent that belongs to bay - name contains bay name
			string [] bayName = ground.name.Split ("-" [0]);
			GameObject agent = GameObject.Find (bayName [0]);
			aI = agent.GetComponent<AI> ();
		}
		// if cube touched goal and agent not dead
		if (other.gameObject.CompareTag("cube") && aI.currentState.ToString() != "Death" ) {
            // Swap ground material for a bit to indicate we scored.
            StartCoroutine(GoalScoredSwapGroundMaterial(goalScoredMaterial, goalScoredMarkerMaterial, 0.5f));
        }
    }

    /// Swap ground material, wait time seconds, then swap back to the regular material.
    public IEnumerator GoalScoredSwapGroundMaterial(Material groundMaterial, Material markerMaterial, float time)
    {
        m_GroundRenderer.material = groundMaterial;
		m_MarkerRenderer.material = markerMaterial;
		yield return new WaitForSeconds(time * 2.0f); // Wait for 0.5 sec
        m_GroundRenderer.material = m_GroundMaterial;
		m_MarkerRenderer.material = m_MarkerMaterial;
	}



}
