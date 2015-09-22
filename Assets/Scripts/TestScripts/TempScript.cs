using UnityEngine;
using System.Collections;

public class TempScript : MonoBehaviour {

	public bool stopTime = false;
	public SpriteRenderer renderer;

	bool useControllerOLD = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GeneralFinder.inputManager.getButtonUp("Interaction"))
		    Debug.Log ("interazione");
		if (GeneralFinder.inputManager.getButtonUp("DestraMenu"))
			Debug.Log ("destraMenu");
		if (GeneralFinder.inputManager.getButtonUp("Test"))
			Debug.Log ("TEst");

		//Debug.Log (GeneralFinder.inputManager.getAxis("AsseTest"));

		//Debug.Log (GeneralFinder.inputManager.getAxis("Sasa"));
		//Debug.Log (Input.GetAxis("Horizontal"));

		Debug.Log ("Horiz" + GeneralFinder.inputManager.getAxis("Horizontal"));
		Debug.Log ("Vert" + GeneralFinder.inputManager.getAxis("Vertical"));

		//Debug.Log (Input.GetButton("D"));


		if (GeneralFinder.cursorHandler.useController != useControllerOLD) {
			renderer.sprite = GeneralFinder.inputManager.getSprite("Jump");
		}

		useControllerOLD = GeneralFinder.cursorHandler.useController;

		//Debug.Log (GeneralFinder.buttonKeyboardMouse.getAxis (ButtonKeyboardMouse.Button.A));
		
	}
}
