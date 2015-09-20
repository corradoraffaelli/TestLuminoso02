using UnityEngine;
using System.Collections;

public class provaciao : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp ("Vertical")) {
			Debug.Log("menu ciaone");
			if(Input.GetAxisRaw("Vertical") > 0.0f) {
				Debug.Log("menu ciaone1");
			}
		}

		if (Input.GetAxis ("Vertical") != 0.0f) {

			Debug.Log("asse vertical != 0");

			if(Input.GetAxis("Vertical") > 0.0f) {
				Debug.Log("asse vertical > 0");
			}
			else {
				Debug.Log("asse vertical <= 0");
			}

		}

		if (Input.GetAxis ("Horizontal") != 0.0f) {
			
			Debug.Log("asse horizontal != 0");
		}
	}
}
