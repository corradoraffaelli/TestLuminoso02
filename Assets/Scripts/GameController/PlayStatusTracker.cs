using UnityEngine;
using System.Collections;

public class PlayStatusTracker : MonoBehaviour {

	public static bool inPlay = true;

	public static bool debugMode;


	public bool inPlayMode = true;

	private float prevTimeScale = 1.0f;
	[Range(0.1f, 30.0f)]
	public float timeScale = 1.0f;
	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

		if (inPlay != inPlayMode) {

			if(inPlayMode) {

				Time.timeScale = 1.0f;

			}
			else {

				Time.timeScale = 0.0f;

			}

			inPlay = inPlayMode;

		}

		if (prevTimeScale != timeScale) {
			Debug.Log("ciaone");
			prevTimeScale = timeScale;

			Time.timeScale = timeScale;

		}

	}
}
