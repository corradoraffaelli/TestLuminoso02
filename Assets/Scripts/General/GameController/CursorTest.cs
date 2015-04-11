using UnityEngine;
using System.Collections;

public class CursorTest : MonoBehaviour {

	GameObject controller;
	CursorHandler CH;

	// Use this for initialization
	void Awake () {
		controller = GameObject.FindGameObjectWithTag ("Controller");
		CH = controller.GetComponent<CursorHandler> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cursorPos = CH.getCursorWorldPosition();
		transform.position = new Vector3 (cursorPos.x, cursorPos.y, transform.position.z);
	}
}
