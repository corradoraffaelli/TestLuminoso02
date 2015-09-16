using UnityEngine;
using System.Collections;

public class ControllerVerifyButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.JoystickButton0))
			Debug.Log ("Button 0");
		if (Input.GetKeyUp (KeyCode.JoystickButton1))
			Debug.Log ("Button 1");
		if (Input.GetKeyUp (KeyCode.JoystickButton2))
			Debug.Log ("Button 2");
		if (Input.GetKeyUp (KeyCode.JoystickButton3))
			Debug.Log ("Button 3");
		if (Input.GetKeyUp (KeyCode.JoystickButton4))
			Debug.Log ("Button 4");
		if (Input.GetKeyUp (KeyCode.JoystickButton5))
			Debug.Log ("Button 5");
		if (Input.GetKeyUp (KeyCode.JoystickButton6))
			Debug.Log ("Button 6");
		if (Input.GetKeyUp (KeyCode.JoystickButton7))
			Debug.Log ("Button 7");
		if (Input.GetKeyUp (KeyCode.JoystickButton8))
			Debug.Log ("Button 8");
		if (Input.GetKeyUp (KeyCode.JoystickButton9))
			Debug.Log ("Button 9");
		if (Input.GetKeyUp (KeyCode.JoystickButton10))
			Debug.Log ("Button 10");
		if (Input.GetKeyUp (KeyCode.JoystickButton11))
			Debug.Log ("Button 11");
		if (Input.GetKeyUp (KeyCode.JoystickButton12))
			Debug.Log ("Button 12");
		if (Input.GetKeyUp (KeyCode.JoystickButton13))
			Debug.Log ("Button 13");
		if (Input.GetKeyUp (KeyCode.JoystickButton14))
			Debug.Log ("Button 14");
		if (Input.GetKeyUp (KeyCode.JoystickButton15))
			Debug.Log ("Button 15");
		if (Input.GetKeyUp (KeyCode.JoystickButton16))
			Debug.Log ("Button 16");
		if (Input.GetKeyUp (KeyCode.JoystickButton17))
			Debug.Log ("Button 17");
		if (Input.GetKeyUp (KeyCode.JoystickButton18))
			Debug.Log ("Button 18");
		if (Input.GetKeyUp (KeyCode.JoystickButton19))
			Debug.Log ("Button 19");

		for (int i = 3; i < 21; i++)
		{
			if ((Input.GetAxis("Axis"+i.ToString()) > 0.7f))
			{
				//if (i != 5)
				Debug.Log ("Axis"+i.ToString()+" pos "+(Input.GetAxis("Axis"+i.ToString())));
			}

			if ((Input.GetAxis("Axis"+i.ToString()) < -0.7f))
			{
				//if (i != 5)
					Debug.Log ("Axis"+i.ToString()+" neg "+(Input.GetAxis("Axis"+i.ToString())));
			}
				
		}



	}
}
