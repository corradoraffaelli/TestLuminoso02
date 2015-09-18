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
	 * -----------TIPI DI ASSI---------
	 */
	
	public enum Axis{Axis3, Axis4, Axis5, Axis6, Axis7, Axis8, Axis9,  Axis10, Axis11, Axis12,
		Axis13, Axis14, Axis15, Axis16, Axis17, Axis18, Axis19, Axis20, Horizontal, Vertical, Default};

	/*
	 * -----------TIPI DI BOTTONI FISICI---------
	 */
	
	public enum PS3Button{X, Quadrato, Cerchio, Triangolo, DPadUp, DPadDown, DPadRight, DPadLeft, R1, R2, L1, L2, Start, Select, RButton, LButton, PS, 
	StickRUp, StickRRight, StickLUp, StickLRight};	

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
			//Button tempButton;
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
	public ButtonElement[] buttonElements;

	[System.Serializable]
	public class AxisElement
	{
		public string scriptInterface;
		public PS3Button physicalButton;
		public Axis axis360Windows;
		public Axis axis360Mac;
		public Axis axisPS3Mac;
		public Axis axisPS4Mac;
		
		//METODI
		public string getStandardName()
		{
			//Axis tempAxis;
			switch (controllerType)
			{
			case ControllerType.Windows360:
				return axis360Windows.ToString();
				break;
			case ControllerType.Mac360:
				return axis360Mac.ToString();
				break;
			case ControllerType.MacPS3:
				return axisPS3Mac.ToString();
				break;
			case ControllerType.MacPS4:
				return axisPS4Mac.ToString();
				break;
			default:
				return scriptInterface;
				break;
			}
		}
	}
	
	[SerializeField]
	public AxisElement[] axisElement;

	public string getUsedButton(string buttonName)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].scriptInterface == buttonName)
			{
				return buttonElements[i].getStandardName();
			}
		}
		return "";
	}

	public string getUsedButton(PS3Button ps3Button)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].physicalButton == ps3Button)
			{
				return buttonElements[i].getStandardName();
			}
		}
		return "";
	}

	public string getUsedAxis(string axisName)
	{
		for (int i = 0; i < axisElement.Length; i++) {
			if (axisElement[i] != null && axisElement[i].scriptInterface == axisName)
			{
				return axisElement[i].getStandardName();
			}
		}
		return "";
	}

	public string getUsedAxis(PS3Button ps3Button)
	{
		for (int i = 0; i < axisElement.Length; i++) {
			if (axisElement[i] != null && axisElement[i].physicalButton == ps3Button)
			{
				return axisElement[i].getStandardName();
			}
		}
		return "";
	}

	void Start () {
		switchControllerType ();
		Debug.Log (controllerType.ToString ());
	}

	public bool getButtonUp(string buttonName)
	{
		if (Input.GetButtonUp (getUsedButton (buttonName)))
			return true;
		else
			return false;
	}

	public bool getButtonDown(string buttonName)
	{
		if (Input.GetButtonDown (getUsedButton (buttonName)))
			return true;
		else
			return false;
	}

	public bool getButton(string buttonName)
	{
		if (Input.GetButton (getUsedButton (buttonName)))
			return true;
		else
			return false;
	}

	public float getAxis(string axisName)
	{
		return (Input.GetAxis (getUsedAxis (axisName)));
	}

	public float getAxisRaw(string axisName)
	{
		return (Input.GetAxisRaw (getUsedAxis (axisName)));
	}

	void switchControllerType()
	{
		if (Input.GetJoystickNames ().Length > 0) {
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
			
	}

	void Update () {
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha1))
		{
			controllerType = ControllerType.Windows360;
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha2))
		{
			controllerType = ControllerType.Mac360;
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha3))
		{
			controllerType = ControllerType.MacPS3;
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha4))
		{
			controllerType = ControllerType.MacPS4;
		}
	}
}
