using UnityEngine;
using System.Collections;

public class InfoOnScreenButton : MonoBehaviour {

	public Sprite buttonDown;
	Sprite buttonUp;

	bool playerColliding = false;
	bool playerCollidingOld = false;

	SpriteRenderer spriteRenderer;

	GameObject father;
	InformativeOnScreen informativeOnScreen;

	AudioHandler audioHandler;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		buttonUp = spriteRenderer.sprite;

		audioHandler = GetComponent<AudioHandler> ();

		father = transform.parent.gameObject;
		if (father != null)
			informativeOnScreen = father.GetComponent<InformativeOnScreen> ();
	}

	void Update () {
		if (playerColliding != playerCollidingOld) {
			if (playerColliding)
			{
				if (audioHandler != null)
					audioHandler.playClipByName("ButtonDown");
				changeButtonGraphic(true);
				showNextSprite();
			}
			else
			{
				if (audioHandler != null)
					audioHandler.playClipByName("ButtonUp");
				changeButtonGraphic(false);
			}
		}

		playerCollidingOld = playerColliding;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			playerColliding = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	void changeButtonGraphic(bool active)
	{
		if (active) {
			spriteRenderer.sprite = buttonDown;
		} else {
			spriteRenderer.sprite = buttonUp;
		}
	}

	void showNextSprite()
	{
		if (informativeOnScreen != null) {
			informativeOnScreen.nextSprite();
		}
	}


}
