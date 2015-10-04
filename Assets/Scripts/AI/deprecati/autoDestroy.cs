using UnityEngine;
using System.Collections;

/// <summary>
/// Auto destroy dei nemici. (DEPRECATO)
/// </summary>

//Dario

public class autoDestroy : MonoBehaviour {

	float tStart = -4.0f;
	float tToDestroy = 4.0f;
	bool started = false;
	// Use this for initialization

	public void setTimer(float timer) {

		if (timer >= 0)
			tToDestroy = timer;

	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!started) {
			started = true;
			tStart = Time.time;
		} 
		else {
			if(Time.time > tStart + tToDestroy)
				Destroy(gameObject);

		}
	}
}
