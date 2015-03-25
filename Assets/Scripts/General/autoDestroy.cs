using UnityEngine;
using System.Collections;

public class autoDestroy : MonoBehaviour {

	float tStart = -4.0f;
	float tToDestroy = 4.0f;
	bool started = false;
	// Use this for initialization
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
