using UnityEngine;
using System.Collections;

public class SimpleAppear : MonoBehaviour {

	public bool enabled = false;

	// Use this for initialization
	void Start () {
		if (enabled)
			GetComponent<SpriteRenderer>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
