using UnityEngine;
using System.Collections;

public class UnlockContent : MonoBehaviour {

	public Sprite unlockedObj;
	public Sprite controllerButton;
	public Sprite keyboardButton;



	public GameObject canvasPlayingUI;
	public PlayingUI playingUI;

	public CursorHandler cursorHandler;
	public InformativeManager informativeMan;

	public int sectionIndexToUnlock;
	public int contentIndexToUnlock;

	bool contentUnlocked = false;

	// Use this for initialization
	void Start () {

		initializeComponents ();

	}

	void initializeComponents() {

		playingUI = UtilFinder._GetComponent<PlayingUI> (canvasPlayingUI);

		cursorHandler = UtilFinder._GetComponentOfGameObjectWithTag<CursorHandler> ("Controller");

		informativeMan = UtilFinder._GetComponentOfGameObjectWithTag<InformativeManager> ("Controller");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player" && !contentUnlocked) {

			setUpPlayingUIForNewContent ();

			informativeMan.c_canShowNewContent (sectionIndexToUnlock, contentIndexToUnlock);

			contentUnlocked = true;

		}


	}

	void setUpPlayingUIForNewContent() {

		Sprite buttonSprite;
		
		if (cursorHandler.useController)
			buttonSprite = controllerButton;
		else
			buttonSprite = keyboardButton;
		
		if (playingUI != null) {
			
			Sprite[] sprites = new Sprite[1];
			sprites [0] = unlockedObj;
			
			playingUI.setSprites (sprites, PlayingUI.UIPosition.UpperRight);
			
			playingUI.setButtonSprite (PlayingUI.UIPosition.UpperRight, buttonSprite);
			
			playingUI.setVerticalButton (PlayingUI.UIPosition.UpperRight, false);
			playingUI.setSpritesSize (PlayingUI.UIPosition.UpperRight, PlayingUI.UISize.Big);
			
			playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);
			
			
			
		}

	}
}
