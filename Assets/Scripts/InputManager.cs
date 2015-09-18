using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool getButtonUp(string button) {

		if (GeneralFinder.cursorHandler.useController) {

			return GeneralFinder.buttonController.getButtonUp(button);

		} else {

			return Input.GetButtonUp(button);

		}
	}

	public bool getButtonDown(string button) {
		
		if (GeneralFinder.cursorHandler.useController) {
			
			return GeneralFinder.buttonController.getButtonDown(button);
			
		} else {
			
			return Input.GetButtonDown(button);
			
		}
	}

	public bool getButton(string button) {
		
		if (GeneralFinder.cursorHandler.useController) {

			return GeneralFinder.buttonController.getButton(button);
			
		} else {
			
			return Input.GetButton(button);
			
		}
	}

	public float getAxis(string axis) {

		if (GeneralFinder.cursorHandler.useController) {
			
			return GeneralFinder.buttonController.getAxis(axis);
			
		} else {
			
			return Input.GetAxis(axis);
			
		}

	}

	public float getAxisRaw(string axis) {
		
		if (GeneralFinder.cursorHandler.useController) {
			
			return GeneralFinder.buttonController.getAxisRaw (axis);
			
		} else {
			
			return Input.GetAxisRaw(axis);

		}
		
	}

}
