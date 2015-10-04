using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce la possibilità di avere più mappature di controller. Oltre che un'interfaccia standard. 
/// </summary>

// Corrado
public class ButtonController : MonoBehaviour {


	//[HideInInspector]
	public bool useController;

	/*
	 * -----------TIPI DI CONTROLLER---------
	 */
	
	public enum ControllerType {Default, Windows360, Mac360, MacPS3, MacPS4};
	public static ControllerType controllerType;

	/*
	 * -----------TIPI DI BOTTONI---------
	 */

	public enum Button{Default, Button0, Button1, Button2, Button3, Button4, Button5, Button6,  Button7, Button8, Button9,
	Button10, Button11, Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19};

	/*
	 * -----------TIPI DI ASSI---------
	 */
	
	public enum Axis{Default, Axis3, Axis4, Axis5, Axis6, Axis7, Axis8, Axis9,  Axis10, Axis11, Axis12,
		Axis13, Axis14, Axis15, Axis16, Axis17, Axis18, Axis19, Axis20, Horizontal, Vertical};

	public enum AxisDirection{Default, Positive, Negative}

	/*
	 * -----------TIPI DI BOTTONI FISICI---------
	 */
	
	public enum PS3Button{Default, X, Quadrato, Cerchio, Triangolo, DPadUp, DPadDown, DPadRight, DPadLeft, R1, R2, L1, L2, Start, Select, RButton, LButton, PS, 
		StickRHorizontal, StickRVertical, StickLHorizontal, StickLVertical,  StickRUp, StickRDown, StickRRight, StickRLeft, 
		StickLUp, StickLDown, StickLRight, StickLLeft};	

	[System.Serializable]
	public class GenericInput{
		public Button button;
		public Axis axis;
		public AxisDirection axisDirection;
		[HideInInspector]
		public float axisValue;
		public float axisValueOld;
	}

	[System.Serializable]
	public class ButtonElement
	{
		public string scriptInterface;
		public PS3Button physicalButton;
		public GenericInput button360Windows;
		public GenericInput button360Mac;
		public GenericInput buttonPS3Mac;
		public GenericInput buttonPS4Mac;
		public Sprite button360Sprite;
		public Sprite buttonPS3Sprite;
		public Sprite buttonPS4Sprite;


		//METODI
		public GenericInput getStandardInput()
		{
			//Button tempButton;
			switch (controllerType)
			{
			case ControllerType.Windows360:
				return button360Windows;
				break;
			case ControllerType.Mac360:
				return button360Mac;
				break;
			case ControllerType.MacPS3:
				return buttonPS3Mac;
				break;
			case ControllerType.MacPS4:
				return buttonPS4Mac;
				break;
			default:
				return new GenericInput();
				break;
			}
		}

		public Sprite getSprite()
		{
			//Button tempButton;
			switch (controllerType)
			{
			case ControllerType.Windows360:
				return buttonPS3Sprite;
				break;
			case ControllerType.Mac360:
				return button360Sprite;
				break;
			case ControllerType.MacPS3:
				return buttonPS3Sprite;
				break;
			case ControllerType.MacPS4:
				return buttonPS4Sprite;
				break;
			default:
				return null;
				break;
			}
		}
		
	}

	[SerializeField]
	public ButtonElement[] buttonElements;

	/*
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
	*/

	public Sprite getSprite(PS3Button physicalInput)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].physicalButton == physicalInput)
			{
				return buttonElements[i].getSprite();
			}
		}
		return new Sprite();
	}

	public Sprite getSprite(string buttonName)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].scriptInterface == buttonName)
			{
				return buttonElements[i].getSprite();
			}
		}
		return new Sprite();
	}

	//i due metodi seguenti ritornano la struttura che contiene i dati dell'input, che sia bottone o asse, a seconda del controller inserito
	public GenericInput getUsedButton(string buttonName)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].scriptInterface == buttonName)
			{
				return buttonElements[i].getStandardInput();
			}
		}
		return new GenericInput();
	}

	public GenericInput getUsedButton(PS3Button physicalInput)
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null && buttonElements[i].physicalButton == physicalInput)
			{
				return buttonElements[i].getStandardInput();
			}
		}
		return new GenericInput();
	}


	public bool getButtonUp(PS3Button inputButton)
	{
		//se è un bottone ritorna il valore dell'input standard, se è un asse, controlla i valori del frame attuale e di quello precedente
		//anche in base all'asse negativo o positivo
		GenericInput actualInput = getUsedButton (inputButton);
		if (actualInput.button != Button.Default) {
			return Input.GetButtonUp (actualInput.button.ToString ());
		} else if (actualInput.axis != Axis.Default) {
			if (actualInput.axisDirection == AxisDirection.Positive || actualInput.axisDirection == AxisDirection.Default)
			{
				if (actualInput.axisValue < 0.5f && actualInput.axisValueOld >= 0.5f)
					return true;
			}
			else if (actualInput.axisDirection == AxisDirection.Negative)
			{
				if (actualInput.axisValue > -0.5f && actualInput.axisValueOld <= -0.5f)
					return true;
			}

		}
		return false;
	}

	public bool getButtonDown(PS3Button inputButton)
	{
		//se è un bottone ritorna il valore dell'input standard, se è un asse, controlla i valori del frame attuale e di quello precedente
		GenericInput actualInput = getUsedButton (inputButton);
		if (actualInput.button != Button.Default) {
			return Input.GetButtonDown (actualInput.button.ToString ());
		} else if (actualInput.axis != Axis.Default) {
			if (actualInput.axisDirection == AxisDirection.Positive || actualInput.axisDirection == AxisDirection.Default)
			{
				if (actualInput.axisValue > 0.5f && actualInput.axisValueOld <= 0.5f)
					return true;
			}
			else if (actualInput.axisDirection == AxisDirection.Negative)
			{
				if (actualInput.axisValue < -0.5f && actualInput.axisValueOld >= -0.5f)
					return true;
			}

		}
		return false;
	}

	public bool getButton(PS3Button inputButton)
	{
		//se è un bottone ritorna il valore dell'input standard, se è un asse, controlla i valori del frame attuale
		GenericInput actualInput = getUsedButton (inputButton);
		if (actualInput.button != Button.Default) {
			return Input.GetButton (actualInput.button.ToString ());
		} else if (actualInput.axis != Axis.Default) {
			if (actualInput.axisDirection == AxisDirection.Positive || actualInput.axisDirection == AxisDirection.Default)
			{
				if (actualInput.axisValue > 0.5f)
					return true;
			}
			else if (actualInput.axisDirection == AxisDirection.Negative)
			{
				if (actualInput.axisValue < -0.5f)
					return true;
			}

		}
		return false;
	}

	public float getAxis(PS3Button inputButton)
	{

		GenericInput actualInput = getUsedButton (inputButton);

		if (actualInput.axis != Axis.Default) {
			//Debug.Log (inputButton.ToString() + " " + Input.GetAxis (actualInput.axis.ToString ()));
			return Input.GetAxis (actualInput.axis.ToString ());
		} else if (actualInput.button != Button.Default) {
			if (Input.GetButton(actualInput.button.ToString()))
			    return 1.0f;
			else
				return 0.0f;
		}

		return 0.0f;
	}

	public float getAxisRaw(PS3Button inputButton)
	{
		
		GenericInput actualInput = getUsedButton (inputButton);
		
		if (actualInput.axis != Axis.Default) {
			return Input.GetAxisRaw (actualInput.axis.ToString ());
		} else if (actualInput.button != Button.Default) {
			if (Input.GetButton(actualInput.button.ToString()))
				return 1.0f;
			else
				return 0.0f;
		}

		return 0.0f;
	}

	void Start () {
		switchControllerType ();
		Debug.Log (controllerType.ToString ());
	}

	//funzione chiamata allo start, a seconda del controller inserito, ne mette il tipo
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


	void updateUseController()
	{
		useController = GeneralFinder.cursorHandler.useController;
	}


	void updateAxisValue()
	{
		for (int i = 0; i < buttonElements.Length; i++) {
			if (buttonElements[i] != null)
			{
				if (buttonElements[i].button360Windows != null && buttonElements[i].button360Windows.axis != Axis.Default)
				{
					buttonElements[i].button360Windows.axisValueOld = buttonElements[i].button360Windows.axisValue;
					buttonElements[i].button360Windows.axisValue = Input.GetAxis(buttonElements[i].button360Windows.axis.ToString());
				}

				if (buttonElements[i].button360Mac != null && buttonElements[i].button360Mac.axis != Axis.Default)
				{
					buttonElements[i].button360Mac.axisValueOld = buttonElements[i].button360Mac.axisValue;
					buttonElements[i].button360Mac.axisValue = Input.GetAxis(buttonElements[i].button360Mac.axis.ToString());
				}

				if (buttonElements[i].buttonPS3Mac != null && buttonElements[i].buttonPS3Mac.axis != Axis.Default)
				{
					buttonElements[i].buttonPS3Mac.axisValueOld = buttonElements[i].buttonPS3Mac.axisValue;
					buttonElements[i].buttonPS3Mac.axisValue = Input.GetAxis(buttonElements[i].buttonPS3Mac.axis.ToString());
				}

				if (buttonElements[i].buttonPS4Mac != null && buttonElements[i].buttonPS4Mac.axis != Axis.Default)
				{
					buttonElements[i].buttonPS4Mac.axisValueOld = buttonElements[i].buttonPS4Mac.axisValue;
					buttonElements[i].buttonPS4Mac.axisValue = Input.GetAxis(buttonElements[i].buttonPS4Mac.axis.ToString());
				}

			}
		}
	}

	void Update () {
		updateAxisValue ();

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

		updateUseController ();
	}

}
