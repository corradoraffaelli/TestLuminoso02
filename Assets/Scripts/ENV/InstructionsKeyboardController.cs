﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Metodo che si occupa di switchare grafica dei bottoni mostrati a schermo, nel caso si utilizzi controller o tastiera
/// </summary>

// Corrado
public class InstructionsKeyboardController : MonoBehaviour {

	public bool sameSprite = false;
	public Sprite keyboardSprite;
	public Sprite controllerSprite;

	public bool overrideKeyboard = false;
	public bool overrideController = false;
	public ButtonController.PS3Button controllerButton;
	public ButtonKeyboardMouse.Button keyboardButton;

	Vector3 normalScale;
	public float controllerScale = 1.0f;
	public float keyboardScale = 1.0f;

	bool useController = false;
	bool wasUseController = false;

	SpriteRenderer spriteRenderer;

	void Start () {
		normalScale = transform.localScale;

		if (keyboardSprite == null)
			keyboardSprite = GeneralFinder.inputManager.getKeyboardSprite (keyboardButton);
		if (controllerSprite == null)
			controllerSprite = GeneralFinder.inputManager.getControllerSprite (controllerButton);

		spriteRenderer = GetComponent<SpriteRenderer>();

		useController = GeneralFinder.cursorHandler.useController;
		setKeyboardSprite ();
	}

	void Update () {
		if (!sameSprite)
		{
			useController = GeneralFinder.cursorHandler.useController;
			if (wasUseController != useController)
				setKeyboardSprite();
			wasUseController = useController;
		}
	}

	void setKeyboardSprite()
	{
		if (spriteRenderer != null)
		{
			if (useController)
			{
				if (overrideController)
					controllerSprite = GeneralFinder.inputManager.getControllerSprite (controllerButton);
				spriteRenderer.sprite = controllerSprite;
				transform.localScale = new Vector3(controllerScale*normalScale.x, controllerScale*normalScale.y, controllerScale*normalScale.z);
			}
			else if (!useController)
			{
				if (overrideKeyboard)
					keyboardSprite = GeneralFinder.inputManager.getKeyboardSprite (keyboardButton);
				spriteRenderer.sprite = keyboardSprite;
				transform.localScale = new Vector3(keyboardScale*normalScale.x, keyboardScale*normalScale.y, keyboardScale*normalScale.z);
			}
		}
	}
}
