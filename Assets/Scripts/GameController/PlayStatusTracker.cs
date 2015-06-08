using UnityEngine;
using System.Collections;

public class PlayStatusTracker : MonoBehaviour {

	public static bool inPlay = true;

	public static bool debugMode;


	public bool inPlayMode = true;
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

	}
}
