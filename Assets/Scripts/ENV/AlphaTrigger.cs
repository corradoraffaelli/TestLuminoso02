using UnityEngine;
using System.Collections;

/// <summary>
/// Se il player entra nel trigger, abilita e disabilita gli oggetti specificati
/// </summary>

// Corrado
public class AlphaTrigger : MonoBehaviour {

	public SpriteRenderer[] objectToEnable;
	public SpriteRenderer[] objectToDisable;

	bool playerColliding = false;
	bool playerCollidingOLD = false;

	public bool switchOnExit = true;

	void Update () {

		if (playerCollidingOLD != playerColliding) {
			if (playerColliding)
			{
				disableSprites(objectToDisable);
				enableSprites(objectToEnable);
				//Debug.Log ("playerEntrato");
			}
			else
			{
				if (switchOnExit)
				{
					disableSprites(objectToEnable);
					enableSprites(objectToDisable);
				}
			}
		}

		playerCollidingOLD = playerColliding;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	void disableSprites(SpriteRenderer[] rendArray)
	{
		for (int i = 0; i < rendArray.Length; i++) {
			if (rendArray[i] != null)
			{
				Color actColor = rendArray[i].color;
				Color newColor = new Color(actColor.r, actColor.g, actColor.b, 0.0f);
				rendArray[i].color = newColor;
			}
		}
	}

	void enableSprites(SpriteRenderer[] rendArray)
	{
		for (int i = 0; i < rendArray.Length; i++) {
			if (rendArray[i] != null)
			{
				Color actColor = rendArray[i].color;
				Color newColor = new Color(actColor.r, actColor.g, actColor.b, 1.0f);
				rendArray[i].color = newColor;
			}
		}
	}
}
