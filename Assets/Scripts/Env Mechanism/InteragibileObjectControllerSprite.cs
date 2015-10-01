using UnityEngine;
using System.Collections;

public class InteragibileObjectControllerSprite : MonoBehaviour {

	public Sprite keyboardSprite;
	public Sprite controllerSprite;
	public string action = "Interaction";

	public bool useController = false;
	bool wasUseController = false;

	SpriteRenderer spriteRenderer;

	public bool overlaySprites = true;

	public float indicationScale = 2.0f;
	public float overlayScale = 0.5f;

	void Start () {
		//wasUseController = GeneralFinder.cursorHandler.useController;
		//Debug.Log ("2");
		useController = GeneralFinder.cursorHandler.useController;
		spriteRenderer = GetComponent<SpriteRenderer>();
		setIndicationScale ();
		updateSprites ();
	}

	void Update () {
		if (spriteRenderer != null)
		{
			useController = GeneralFinder.cursorHandler.useController;
			
			if (wasUseController != useController)
			{
				setIndicationScale();
				updateSprites();
			}
			
			wasUseController = useController;
		}

	}

	void updateSprites()
	{
		if (overlaySprites)
		{
			keyboardSprite = GeneralFinder.inputManager.getSprite(action);
			controllerSprite = GeneralFinder.inputManager.getSprite(action);
		}
		
		if (keyboardSprite != null && controllerSprite != null)
		{
			if (useController)
			{
				spriteRenderer.sprite = controllerSprite;
			}
			else
			{
				spriteRenderer.sprite = keyboardSprite;
			}

		}
	}

	public void setIndicationScale()
	{
		Transform actualParent = transform.parent;
		transform.parent = null;
		if (overlaySprites)
			transform.localScale = new Vector3 (indicationScale*overlayScale, indicationScale*overlayScale, indicationScale*overlayScale);
		else
			transform.localScale = new Vector3 (indicationScale, indicationScale, indicationScale);
		transform.parent = actualParent;
	}
}
