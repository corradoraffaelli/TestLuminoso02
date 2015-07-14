using UnityEngine;
using System.Collections;

public class InstructionsKeyboardController : MonoBehaviour {

	public bool sameSprite = false;
	public Sprite keyboardSprite;
	public Sprite controllerSprite;

	bool useController = false;
	bool wasUseController = false;

	SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
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
			if (useController && controllerSprite != null)
			{
				spriteRenderer.sprite = controllerSprite;
			}
			else if (!useController && keyboardSprite != null)
			{
				spriteRenderer.sprite = keyboardSprite;
			}
		}
	}
}
