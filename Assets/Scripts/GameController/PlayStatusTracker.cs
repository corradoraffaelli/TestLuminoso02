using UnityEngine;
using System.Collections;

public class PlayStatusTracker : MonoBehaviour {

	private static bool prevInPlay = true;
	public static bool inPlay = true;

	public static bool debugMode;


	public bool triggerStartStopTime = false;

	private float prevTimeScale = 1.0f;
	[Range(0.0f, 30.0f)]
	public float timeScale = 1.0f;
	// Use this for initialization
	void Start () {


	}

	public static void stopTime() {

		PlayStatusTracker.inPlay = false;

	}

	public static void startTime() {
		
		PlayStatusTracker.inPlay = true;
		
	}
	
	// Update is called once per frame
	void Update () {

		if (triggerStartStopTime) {

			inPlay = !inPlay;
			triggerStartStopTime = false;

		}

		if (inPlay != prevInPlay) {

			if (inPlay) {

				Time.timeScale = 1.0f;
				prevTimeScale = 1.0f;
				timeScale = 1.0f;
			} else {

				Time.timeScale = 0.0f;
				prevTimeScale = 0.0f;
				timeScale = 0.0f;
			}

			prevInPlay = inPlay;

		} 
		else {
			if (prevTimeScale != timeScale) {
				Debug.Log ("ciaone");
				prevTimeScale = timeScale;

				Time.timeScale = timeScale;

			}
		}
	}
}
