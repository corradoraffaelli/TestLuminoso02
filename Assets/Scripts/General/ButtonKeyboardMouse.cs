using UnityEngine;
using System.Collections;

public class ButtonKeyboardMouse : MonoBehaviour {

	/*
	 * -----------TIPI DI BOTTONI---------
	 */
	
	public enum Button{Default, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, ESC, RETURN, BACK, SPACE, LeftCTRL, LeftSHIFT, 
		TAB, LeftALT, RightCTRL, RightALT, RightSHIFT, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9, Num0, LeftMouse, RightMouse, ArrowUp,
		ArrowDown, ArrowRight, ArrowLeft, Horizontal, Vertical, ENTER};

	public enum Axis{Default, Horizontal, Vertical};

	public enum AxisDirection{Default, Positive, Negative}

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
	public class KeyboardElement
	{
		public string scriptInterface;
		public GenericInput genericInput;
		public Sprite buttonSprite;	
		public Sprite buttonSpriteAlt;	
		
		//METODI
		public GenericInput getStandardInput()
		{
			return genericInput;
		}
		
		public Sprite getSprite()
		{
			return buttonSprite;
		}

		public Sprite getSpriteAlt()
		{
			return buttonSpriteAlt;
		}
		
	}
	
	[SerializeField]
	public KeyboardElement[] keyboardElements;

	public Sprite getSprite(string buttonName, bool altSprite = false)
	{
		for (int i = 0; i < keyboardElements.Length; i++) {
			if (keyboardElements[i] != null && keyboardElements[i].scriptInterface == buttonName)
			{
				if (!altSprite)
					return keyboardElements[i].getSprite();
				else
					return keyboardElements[i].getSpriteAlt();
			}
		}
		return new Sprite();
	}

	public Sprite getSprite(Button buttonType, bool altSprite = false)
	{
		for (int i = 0; i < keyboardElements.Length; i++) {
			if (keyboardElements[i] != null && keyboardElements[i].genericInput.button == buttonType)
			{
				if (!altSprite)
					return keyboardElements[i].getSprite();
				else
					return keyboardElements[i].getSpriteAlt();
			}
		}
		return new Sprite();
	}

	//i due metodi seguenti ritornano la struttura che contiene i dati dell'input, che sia bottone o asse, a seconda del controller inserito
	public GenericInput getUsedButton(string buttonName)
	{
		for (int i = 0; i < keyboardElements.Length; i++) {
			if (keyboardElements[i] != null && keyboardElements[i].scriptInterface == buttonName)
			{
				return keyboardElements[i].getStandardInput();
			}
		}
		return new GenericInput();
	}

	public GenericInput getUsedButton(Button buttonType)
	{
		for (int i = 0; i < keyboardElements.Length; i++) {
			if (keyboardElements[i] != null && keyboardElements[i].genericInput.button == buttonType)
			{
				return keyboardElements[i].getStandardInput();
			}
		}
		return new GenericInput();
	}


	public bool getButtonUp(Button inputButton)
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
	
	public bool getButtonDown(Button inputButton)
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
	
	public bool getButton(Button inputButton)
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
	
	public float getAxis(Button inputButton)
	{
		//Debug.Log (inputButton.ToString ());
		GenericInput actualInput = getUsedButton (inputButton);
		
		if (actualInput.axis != Axis.Default) {
			if (inputButton == Button.Horizontal)
			{
				return Input.GetAxis ("Horizontal");
			}
			if (inputButton == Button.Vertical)
			{
				return Input.GetAxis ("Vertical");
			}
			return Input.GetAxis (actualInput.axis.ToString ());
		} else if (actualInput.button != Button.Default) {
			if (Input.GetButton(actualInput.button.ToString()))
				return 1.0f;
			else
				return 0.0f;
		}
		
		return 0.0f;
	}
	
	public float getAxisRaw(Button inputButton)
	{
		
		GenericInput actualInput = getUsedButton (inputButton);
		
		if (actualInput.axis != Axis.Default) {
			if (inputButton == Button.Horizontal)
			{
				return Input.GetAxisRaw ("Horizontal");
			}
			if (inputButton == Button.Vertical)
			{
				return Input.GetAxisRaw ("Vertical");
			}
			return Input.GetAxisRaw (actualInput.axis.ToString ());
		} else if (actualInput.button != Button.Default) {
			if (Input.GetButton(actualInput.button.ToString()))
				return 1.0f;
			else
				return 0.0f;
		}
		
		return 0.0f;
	}

	void updateAxisValue()
	{
		for (int i = 0; i < keyboardElements.Length; i++) {
			if (keyboardElements[i] != null)
			{
				if (keyboardElements[i].genericInput.axis != Axis.Default)
				{
					keyboardElements[i].genericInput.axisValueOld = keyboardElements[i].genericInput.axisValue;
					keyboardElements[i].genericInput.axisValue = Input.GetAxis(keyboardElements[i].genericInput.axis.ToString());
				}
			}
		}
	}

	void Start () {
	
	}

	void Update () {
		updateAxisValue ();
	}
}
