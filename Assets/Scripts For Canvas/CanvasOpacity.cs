using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOpacity : MonoBehaviour
{
    private float StartTime;
    private float Opacity;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        Opacity = 0f;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer() > 2.0f)
        {
            // change canvas opacity
            Opacity += 0.1f;
            Debug.Log(Opacity);
            canvasGroup.alpha = Opacity;
        }
    }

    int Timer () {
		// capture time elapsed
		int TimeElapsed = Mathf.RoundToInt(Time.time - StartTime);
		// send time to animation controller
		
        return TimeElapsed;
	}
}
