using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGroundMaterial : MonoBehaviour
{
    public GameObject ground;
    Material m_GroundMaterial; //cached on Awake()
    public Material goalScoredMaterial;
    Renderer m_GroundRenderer;
    GoalDetect GoalDetect;

    // Start is called before the first frame update
    void Start()
    {
        // Get the ground renderer so we can change the material when a goal is scored
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;
        // get goal detect script
        GoalDetect = GetComponent<GoalDetect>();
    }

    void OnTriggerEnter(Collider other) 
    {
        // Touched goal and goal name
        if (other.gameObject.CompareTag("cube")) {
            // Swap ground material for a bit to indicate we scored.
            StartCoroutine(GoalScoredSwapGroundMaterial(goalScoredMaterial, 0.5f));
        }
    }

    /// Swap ground material, wait time seconds, then swap back to the regular material.
    public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }
}
