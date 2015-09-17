using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	/*
	 * -----------TIPI DI CONTROLLER---------
	 */
	
	public enum ControllerType { Windows360, Mac360, MacPS3, MacPS4};
	public static ControllerType controllerType;

	/*
	 * -----------TIPI DI BOTTONI---------
	 */

	public enum Button{Button0, Button1, Button2, Button3, Button4, Button5, Button6,  Button7, Button8, Button9,
	Button10, Button11, Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19, Default};

	/*
	 * -----------TIPI DI BOTTONI FISICI---------
	 */
	
	public enum PS3Button{X, Quadrato, Cerchio, Triangolo, DPadUp, DPadDown, DPadRight, DPadLeft, R1, R2, L1, L2, Start, Select, RButton, LButton, PS};	

	[System.Serializable]
	public class ButtonElement
	{
		public string scriptInterface;
		public PS3Button physicalButton;
		public Button button360Windows;
		public Button button360Mac;
		public Button buttonPS3Mac;
		public Button buttonPS4Mac;

		//METODI
		public string getStandardName()
		{
			Button tempButton;
			switch (controllerType)
			{
			case ControllerType.Windows360:
				return button360Windows.ToString();
				break;
			case ControllerType.Mac360:
				return button360Mac.ToString();
				break;
			case ControllerType.MacPS3:
				return buttonPS3Mac.ToString();
				break;
			case ControllerType.MacPS4:
				return buttonPS4Mac.ToString();
				break;
			default:
				return scriptInterface;
				break;
			}
		}
	}

	[SerializeField]
	ButtonElement[] buttonElements;

	public string getButtonStandard(string buttonName)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].scriptInterface == buttonName)
			{
				return buttonElements[i].getStandardName();
			}
		}
		return "";
	}

	public string getButtonStandard(PS3Button ps3Button)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].physicalButton == ps3Button)
			{
				return buttonElements[i].getStandardName();
			}
		}
		return "";
	}

	void Start () {
		switchControllerType ();
		Debug.Log (controllerType.ToString ());
	}

	void switchControllerType()
	{
		string joystickName = Input.GetJoystickNames () [0];

		if (joystickName.Contains ("360") && joystickName.Contains ("Windows"))
			controllerType = ControllerType.Windows360;
		if (joystickName.Contains ("360") && !joystickName.Contains ("Windows"))
			controllerType = ControllerType.Mac360;
		if (joystickName.Contains ("Sony") && joystickName.Contains ("3"))
			controllerType = ControllerType.MacPS3;
		if (joystickName.Contains ("Sony") && !joystickName.Contains ("3"))
			controllerType = ControllerType.MacPS4;
	}

	void Update () {
		//Debug.Log (interactionElements [0].button360Mac.ToString ());
		//if (Input.GetButtonUp ("Button0"))
		//	Debug.Log ("ppipipipi");
	}
}
