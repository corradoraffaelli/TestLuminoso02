using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	[System.Serializable]
	public class ButtonInput{
		public string interfaceName;
		public string alternativeInterfaceName;
		public ButtonController.PS3Button[] controllerButtons;
		public ButtonKeyboardMouse.Button[] keyboardButtons;
	}

	[System.Serializable]
	public class AxisInput{
		public string interfaceName;
		public string alternativeInterfaceName;
		public AxisControllerComp[] controllerAxis;
		public AxisKeyboardComp[] keyboardAxis;	
	}

	[System.Serializable]
	public class AxisControllerComp
	{
		public ButtonController.PS3Button controllerPositiveAxis;
		public ButtonController.PS3Button controllerNegativeAxis;
		public bool invertNegative = false;
	}

	[System.Serializable]
	public class AxisKeyboardComp
	{
		public ButtonKeyboardMouse.Button keyboardPositiveAxis;		
		public ButtonKeyboardMouse.Button keyboardNegativeAxis;
		public bool invertNegative = false;
	}

	public ButtonInput[] buttons;
	public AxisInput[] axis;

	public bool getButtonUp(string buttonName)
	{
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons[i] != null && (buttons[i].interfaceName == buttonName || buttons[i].alternativeInterfaceName == buttonName))
			{
				if (GeneralFinder.buttonController.useController)
				{
					//Debug.Log ("uso controller");
					for (int j = 0; j < buttons[i].controllerButtons.Length; j++)
					{
						if (buttons[i].controllerButtons[j] != null && GeneralFinder.buttonController.getButtonUp(buttons[i].controllerButtons[j]))
							return true;
					}
				}

				else
				{
					for (int j = 0; j < buttons[i].keyboardButtons.Length; j++)
					{
						if (buttons[i].keyboardButtons[j] != null && GeneralFinder.buttonKeyboardMouse.getButtonUp(buttons[i].keyboardButtons[j]))
							return true;
					}
				}

				break;
			}
		}
		return false;
	}

	public bool getButtonDown(string buttonName)
	{
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons[i] != null && (buttons[i].interfaceName == buttonName || buttons[i].alternativeInterfaceName == buttonName))
			{
				if (GeneralFinder.buttonController.useController)
				{
					//Debug.Log ("uso controller");
					for (int j = 0; j < buttons[i].controllerButtons.Length; j++)
					{
						if (buttons[i].controllerButtons[j] != null && GeneralFinder.buttonController.getButtonDown(buttons[i].controllerButtons[j]))
							return true;
					}
				}
				
				else
				{
					for (int j = 0; j < buttons[i].keyboardButtons.Length; j++)
					{
						if (buttons[i].keyboardButtons[j] != null && GeneralFinder.buttonKeyboardMouse.getButtonDown(buttons[i].keyboardButtons[j]))
							return true;
					}
				}
				
				break;
			}
		}
		return false;
	}

	public bool getButton(string buttonName)
	{
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons[i] != null && (buttons[i].interfaceName == buttonName || buttons[i].alternativeInterfaceName == buttonName))
			{
				if (GeneralFinder.buttonController.useController)
				{
					//Debug.Log ("uso controller");
					for (int j = 0; j < buttons[i].controllerButtons.Length; j++)
					{
						if (buttons[i].controllerButtons[j] != null && GeneralFinder.buttonController.getButton(buttons[i].controllerButtons[j]))
							return true;
					}
				}
				
				else
				{
					for (int j = 0; j < buttons[i].keyboardButtons.Length; j++)
					{
						if (buttons[i].keyboardButtons[j] != null && GeneralFinder.buttonKeyboardMouse.getButton(buttons[i].keyboardButtons[j]))
							return true;
					}
				}
				
				break;
			}
		}
		return false;
	}

	public float getAxisRaw(string axisName)
	{
		for (int i = 0; i < buttons.Length; i++) {
			if (axis[i] != null && (axis[i].interfaceName == axisName || axis[i].alternativeInterfaceName == axisName))
			{
				if (GeneralFinder.buttonController.useController)
				{

					//Debug.Log ("uso controller");
					for (int j = 0; j < axis[i].controllerAxis.Length; j++)
					{
						if (axis[i].controllerAxis[j] != null && axis[i].controllerAxis[j].controllerNegativeAxis != null && axis[i].controllerAxis[j].controllerPositiveAxis!= null)
						{
							float tempValue = 0.0f;
							if (axis[i].controllerAxis[j].controllerNegativeAxis != ButtonController.PS3Button.Default)
							{
								if (!axis[i].controllerAxis[j].invertNegative)
									tempValue += GeneralFinder.buttonController.getAxisRaw(axis[i].controllerAxis[j].controllerNegativeAxis);
								else
									tempValue -= GeneralFinder.buttonController.getAxisRaw(axis[i].controllerAxis[j].controllerNegativeAxis);
							}
								
							if (axis[i].controllerAxis[j].controllerPositiveAxis != ButtonController.PS3Button.Default)
								tempValue += GeneralFinder.buttonController.getAxisRaw(axis[i].controllerAxis[j].controllerPositiveAxis);

							return tempValue;
						}
					}
				}
				
				else
				{
					for (int j = 0; j < axis[i].keyboardAxis.Length; j++)
					{
						if (axis[i].keyboardAxis[j] != null && axis[i].keyboardAxis[j].keyboardNegativeAxis != null && axis[i].keyboardAxis[j].keyboardPositiveAxis!= null)
						{
							float tempValue = 0.0f;
							if (axis[i].keyboardAxis[j].keyboardNegativeAxis != ButtonKeyboardMouse.Button.Default)
							{
								if (!axis[i].keyboardAxis[j].invertNegative)
									tempValue += GeneralFinder.buttonKeyboardMouse.getAxisRaw(axis[i].keyboardAxis[j].keyboardNegativeAxis);
								else
									tempValue -= GeneralFinder.buttonKeyboardMouse.getAxisRaw(axis[i].keyboardAxis[j].keyboardNegativeAxis);
							}
								
							if (axis[i].keyboardAxis[j].keyboardPositiveAxis != ButtonKeyboardMouse.Button.Default)
								tempValue += GeneralFinder.buttonKeyboardMouse.getAxisRaw(axis[i].keyboardAxis[j].keyboardPositiveAxis);
							
							return tempValue;
						}
					}
				}
				
				break;
			}
		}
		//return false;
		return 0.0f;
	}

	public float getAxis(string axisName)
	{
		for (int i = 0; i < buttons.Length; i++) {
			if (axis[i] != null && (axis[i].interfaceName == axisName || axis[i].alternativeInterfaceName == axisName))
			{
				if (GeneralFinder.buttonController.useController)
				{
					
					//Debug.Log ("uso controller");
					for (int j = 0; j < axis[i].controllerAxis.Length; j++)
					{
						if (axis[i].controllerAxis[j] != null && axis[i].controllerAxis[j].controllerNegativeAxis != null && axis[i].controllerAxis[j].controllerPositiveAxis!= null)
						{
							float tempValue = 0.0f;
							if (axis[i].controllerAxis[j].controllerNegativeAxis != ButtonController.PS3Button.Default)
							{
								if (!axis[i].controllerAxis[j].invertNegative)
									tempValue += GeneralFinder.buttonController.getAxis(axis[i].controllerAxis[j].controllerNegativeAxis);
								else
									tempValue -= GeneralFinder.buttonController.getAxis(axis[i].controllerAxis[j].controllerNegativeAxis);
							}
							
							if (axis[i].controllerAxis[j].controllerPositiveAxis != ButtonController.PS3Button.Default)
								tempValue += GeneralFinder.buttonController.getAxis(axis[i].controllerAxis[j].controllerPositiveAxis);
							
							return tempValue;
						}
					}
				}
				
				else
				{
					for (int j = 0; j < axis[i].keyboardAxis.Length; j++)
					{
						if (axis[i].keyboardAxis[j] != null && axis[i].keyboardAxis[j].keyboardNegativeAxis != null && axis[i].keyboardAxis[j].keyboardPositiveAxis!= null)
						{
							float tempValue = 0.0f;
							if (axis[i].keyboardAxis[j].keyboardNegativeAxis != ButtonKeyboardMouse.Button.Default)
							{
								if (!axis[i].keyboardAxis[j].invertNegative)
									tempValue += GeneralFinder.buttonKeyboardMouse.getAxis(axis[i].keyboardAxis[j].keyboardNegativeAxis);
								else
									tempValue -= GeneralFinder.buttonKeyboardMouse.getAxis(axis[i].keyboardAxis[j].keyboardNegativeAxis);
							}
							
							if (axis[i].keyboardAxis[j].keyboardPositiveAxis != ButtonKeyboardMouse.Button.Default)
								tempValue += GeneralFinder.buttonKeyboardMouse.getAxis(axis[i].keyboardAxis[j].keyboardPositiveAxis);
							
							return tempValue;
						}
					}
				}
				
				break;
			}
		}
		//return false;
		return 0.0f;
	}

	public Sprite getSprite(string buttonName)
	{
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons[i] != null && (buttons[i].interfaceName == buttonName || buttons[i].alternativeInterfaceName == buttonName))
			{
				if (GeneralFinder.buttonController.useController)
				{
					//Debug.Log ("uso controller");
					for (int j = 0; j < buttons[i].controllerButtons.Length; j++)
					{
						if (buttons[i].controllerButtons[j] != null)
							return GeneralFinder.buttonController.getSprite (buttons[i].controllerButtons[j]);
					}
				}
				
				else
				{
					for (int j = 0; j < buttons[i].keyboardButtons.Length; j++)
					{
						if (buttons[i].keyboardButtons[j] != null)
							return GeneralFinder.buttonKeyboardMouse.getSprite (buttons[i].keyboardButtons[j]);
					}
				}
				
				break;
			}
		}

		return new Sprite ();

		/*
		if (GeneralFinder.cursorHandler.useController) {
			return GeneralFinder.buttonController.getSprite (buttonName);
		} else {
			return GeneralFinder.buttonKeyboardMouse.getSprite (buttonName);
		}
		*/
}

	public Sprite getControllerSprite(ButtonController.PS3Button inputButton){
		return GeneralFinder.buttonController.getSprite (inputButton);
	}

	public Sprite getKeyboardSprite(ButtonKeyboardMouse.Button inputButton){
		return GeneralFinder.buttonKeyboardMouse.getSprite (inputButton);
	}

	/*
	void Start () {
		
	}

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
	*/

}
