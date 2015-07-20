using UnityEngine;
using System.Collections;

public class InteragibileObjectControllerSprite : MonoBehaviour {

	public Sprite keyboardSprite;
	public Sprite controllerSprite;

	bool wasUseController = false;

	SpriteRenderer spriteRenderer;

	void Start () {
		//wasUseController = GeneralFinder.cursorHandler.useController;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update () {
		if (spriteRenderer != null)
		{
			bool useController = GeneralFinder.cursorHandler.useController;
			
			if (wasUseController != useController)
			{
				if (keyboardSprite != null && controllerSprite != null)
				{
					if (useController)
						spriteRenderer.sprite = controllerSprite;
					else
						spriteRenderer.sprite = keyboardSprite;
						
				}
			}
			
			wasUseController = useController;
		}

	}
}
