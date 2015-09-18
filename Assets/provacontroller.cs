using UnityEngine;
using System.Collections;

public class provacontroller : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GeneralFinder.inputManager.getButtonDown("Interaction")) {
			Debug.Log("interazione premuta");
		}
	}
}
