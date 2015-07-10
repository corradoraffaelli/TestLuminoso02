using UnityEngine;
using System.Collections;

public class LibraRopeDisappear : MonoBehaviour {

	public GameObject wheel;
	SpriteRenderer wheelSpriteRenderer;
	SpriteRenderer spriteRenderer;

	bool wasOver = false;

	void Start () {
		if (wheel != null)
			wheelSpriteRenderer = wheel.GetComponent<SpriteRenderer>();

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update () {
		bool isOver = verifyIfOver();
		if (wasOver != isOver)
		{
			hideRenderer(isOver);
			wasOver = isOver;
		}
	}

	bool verifyIfOver()
	{
		if (spriteRenderer!= null && wheelSpriteRenderer!= null)
		{
			if (spriteRenderer.bounds.center.y > wheelSpriteRenderer.bounds.center.y)
				return true;
			else
				return false;
		}
		return false;
	}

	void hideRenderer(bool hide)
	{
		if (spriteRenderer != null)
		{
			Color actColor = spriteRenderer.color;
			float newAlpha = 0.0f;
			if (!hide)
				newAlpha = 1.0f;
			spriteRenderer.color = new Color(actColor.r, actColor.g, actColor.b, newAlpha);
		}
	}
}
