using UnityEngine;
using System.Collections;

public class debugController : MonoBehaviour {

	[Range(0.01f,1.0f)]
	public float debugTimeScale = 1.0f;
	private float prevTimeScale = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (debugTimeScale != prevTimeScale) {
			Time.timeScale = debugTimeScale;
			prevTimeScale = debugTimeScale;
		}

	}
}
