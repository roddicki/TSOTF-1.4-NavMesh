using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathColor : MonoBehaviour
{

	private GameObject marker;
	private Color deathColor;
	private float duration;

	private void Start ()
	{
		// death color
		deathColor = new Color (0.5f, 0.5f, 0.5f, 1.0f);
		// duration of color change
		duration = 7.0f;
	}


	// initiate death colour in agents with 'duration'
	// this coroutine is run from state.cs (which cannot run coroutines)
	public IEnumerator IniateDeathColor (GameObject m_Ragdoll, string agentName)
	{
		// get ragdoll model
		GameObject m_RagdollModel = m_Ragdoll.transform.Find ("shadow_human_rigged_001_geo").gameObject;
		Renderer m_RagdollRenderer = m_RagdollModel.GetComponent<SkinnedMeshRenderer> ();
		Color ragDollColor = m_RagdollRenderer.GetComponent<Renderer> ().material.color;
		// get bays by tag 'bay'
		GameObject [] bays = GameObject.FindGameObjectsWithTag ("bay");
		foreach (GameObject bay in bays)
		{
			// iterate through to find bay
			if (bay.name.Contains(agentName))
			{
				// get marker model in bay
				marker = bay.transform.Find ("marker").gameObject;
				Color markerColor = marker.GetComponent<Renderer> ().material.color;
			}
		}

		float t = 0;
		// Transition until 0.75 complete leave the agents with some color
		while (t < 0.75f) {
			yield return null;
			t += Time.deltaTime / duration; // Divided by 5 to make it 5 seconds.
			m_RagdollRenderer.GetComponent<Renderer> ().material.color = Color.Lerp (ragDollColor, deathColor, t);
		}
		while (t < 0.99f) {
			yield return null;
			t += Time.deltaTime / duration; // Divided by 5 to make it 5 seconds.
			marker.GetComponent<Renderer> ().material.color = Color.Lerp (ragDollColor, deathColor, t);
		}
	}
}
