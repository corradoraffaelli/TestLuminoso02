using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il comportamento delle leve.
/// </summary>

// Corrado
public class Lever : MonoBehaviour {

	public Sprite leftSprite;
	public Sprite rightSprite;

	SpriteRenderer spriteRenderer;

	InteragibileObject interagibileScript;

	public bool returnOneInteraction = false;
	public bool returnMultipleInteraction = true;

	public float timeToReturn = 0.5f;
	float activationTime;

	bool actualOnLeft = true;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		interagibileScript = GetComponent<InteragibileObject>();
		setBaseSprite();
	}

	void Update () {
		handleReturn();
	}

	public void InteractingMethod()
	{
		//Debug.Log("attivata leva yaya");

		if (interagibileScript != null)
		{
			if (returnOneInteraction && interagibileScript.oneTimeInteraction)
			{
				activationTime = Time.time;
				actualOnLeft = false;
				setBaseSprite (false);
			}
			else if (returnMultipleInteraction && !interagibileScript.oneTimeInteraction)
			{
				activationTime = Time.time;
				actualOnLeft = false;
				setBaseSprite (false);
			}
			else if (!returnOneInteraction && interagibileScript.oneTimeInteraction)
			{
				actualOnLeft = !actualOnLeft;
				setBaseSprite(actualOnLeft);
			}
			else if (!returnMultipleInteraction && !interagibileScript.oneTimeInteraction)
			{
				actualOnLeft = !actualOnLeft;
				setBaseSprite(actualOnLeft);
			}
		}
		else
		{
			actualOnLeft = !actualOnLeft;
			setBaseSprite(actualOnLeft);
		}
	}

	void handleReturn()
	{
		if (interagibileScript != null)
		{
			if (returnOneInteraction && interagibileScript.oneTimeInteraction && !actualOnLeft
			    && (Time.time - activationTime) > timeToReturn)
			{
				actualOnLeft = true;
				setBaseSprite (true);
			}
			else if (returnMultipleInteraction && !interagibileScript.oneTimeInteraction && !actualOnLeft
			         && (Time.time - activationTime) > timeToReturn)
			{
				//activationTime = Time.time;
				actualOnLeft = true;
				setBaseSprite (true);
			}
		}
	}

	void setBaseSprite(bool baseSprite = true)
	{
		if (leftSprite!= null && rightSprite != null && spriteRenderer!= null)
		{
			if (baseSprite)
				spriteRenderer.sprite = leftSprite;
			else
				spriteRenderer.sprite = rightSprite;
		}
	}
}
