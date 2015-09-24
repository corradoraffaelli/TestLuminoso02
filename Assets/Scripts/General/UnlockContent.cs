using UnityEngine;
using System.Collections;

public class UnlockContent : MonoBehaviour {

	public Sprite unlockedObj;
	public Sprite controllerButton;
	public Sprite keyboardButton;

	bool needButtonPress = false;

	CursorHandler cursorHandler;

	public int sectionIndexToUnlock = -1;
	public int contentIndexToUnlock = -1;
	
	public bool disableGameObjectIfTaken = true;

	bool contentUnlocked = false;

	public bool ContentUnlocked {
		
		get{ return contentUnlocked; }

	}

	// Use this for initialization
	void Start () {

		initializeComponents ();

		checkNeedButtonPress ();

		if (sectionIndexToUnlock == -1 || contentIndexToUnlock == -1) {
			
			getIndexes();
			
		}

	}

	void getIndexes() {

		infoContentType tipo;

		if (needButtonPress) {

			tipo = infoContentType.Collectibles;

		} 
		else {

			tipo = infoContentType.FunFacts;

		}

		GeneralFinder.informativeManager.c_getIndexes (this.gameObject, ref sectionIndexToUnlock, ref contentIndexToUnlock, tipo);

	}

	void checkNeedButtonPress() {

		InteragibileObject io = GetComponent<InteragibileObject> ();

		if (io != null) {

			needButtonPress = true;

		} else {

			needButtonPress = false;

		}

	}

	void initializeComponents() {

		cursorHandler = UtilFinder._GetComponentOfGameObjectWithTag<CursorHandler> ("Controller");

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void getCollectible() {

		if (!contentUnlocked) {

			GeneralFinder.informativeManager.c_canShowNewContent (sectionIndexToUnlock, contentIndexToUnlock);

			c_contentUnlocked();

		}


	}

	void OnTriggerEnter2D(Collider2D c) {

		if (!needButtonPress) {

			if(c.tag=="Player") {

				getCollectible();

			}

		}

	}

	void disableThisObject() {

		/*
		SpriteRenderer []srs = GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer sr in srs) {
			
			sr.enabled = false;
			
		}
		*/
		gameObject.SetActive(false);

	}

	void setUpPlayingUIForNewContent() {

		Sprite buttonSprite;
		
		if (cursorHandler.useController)
			buttonSprite = controllerButton;
		else
			buttonSprite = keyboardButton;
		
		if (GeneralFinder.playingUI != null) {
			
			Sprite[] sprites = new Sprite[1];
			sprites [0] = unlockedObj;

			//TODO: prendere le sprite da informativeMangager

			GeneralFinder.playingUI.setSprites (sprites, PlayingUI.UIPosition.UpperRight);
			
			GeneralFinder.playingUI.setButtonSprite (PlayingUI.UIPosition.UpperRight, buttonSprite);
			
			GeneralFinder.playingUI.setVerticalButton (PlayingUI.UIPosition.UpperRight, false);
			GeneralFinder.playingUI.setSpritesSize (PlayingUI.UIPosition.UpperRight, PlayingUI.UISize.Big);
			
			GeneralFinder.playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);
			
			
			
		}

	}


	public void c_setSectionInt(int sec) {

		sectionIndexToUnlock = sec;

	}

	public void c_setContentInt(int con) {

		contentIndexToUnlock = con;
		
	}

	public void c_contentUnlocked() {

		contentUnlocked = true;

		if (disableGameObjectIfTaken) {

			disableThisObject();

		}
		else {


		}

	}

}
