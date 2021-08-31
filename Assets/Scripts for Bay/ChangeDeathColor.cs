using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDeathColor : MonoBehaviour {
	public GameObject marker;
	private Color markerColor;


	private void Start ()
	{
		markerColor = marker.GetComponent<Renderer> ().material.color;
		StartCoroutine (DeathColor ());
	}


	// change color on death
	IEnumerator DeathColor ()
	{
		Debug.Log ("changing color");
		Color currentColour = marker.GetComponent<Renderer> ().material.color;
		yield return new WaitForSeconds (1);
		// death color
		Color deathColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);

		float t = 0;
		float duration = 5.0f;
		// Transition until 0.75 complete
		while (t < 0.85f) {
			yield return null;
			t += Time.deltaTime / duration; // Divided by 5 to make it 5 seconds.
			marker.GetComponent<Renderer> ().material.color = Color.Lerp (markerColor, deathColor, t);
		}

	}
}
