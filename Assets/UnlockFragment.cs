using UnityEngine;
using System.Collections;

public class UnlockFragment : MonoBehaviour {
	
	public Sprite unlockedObj;
	public Sprite controllerButton;
	public Sprite keyboardButton;

	CursorHandler cursorHandler;
	InformativeManager informativeMan;
	
	public int sectionIndexToUnlock = -1;
	public int fragmentIndexToUnlock = -1;
	
	bool fragmentUnlocked = false;
	
	// Use this for initialization
	void Start () {
		
		initializeComponents ();

	}
	

	
	void initializeComponents() {
		
		cursorHandler = UtilFinder._GetComponentOfGameObjectWithTag<CursorHandler> ("Controller");
		
		informativeMan = UtilFinder._GetComponentOfGameObjectWithTag<InformativeManager> ("Controller");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void getFragment() {
		
		if (!fragmentUnlocked) {


			//GeneralFinder.playingUILateral

			//informativeMan.c_canShowNewContent (sectionIndexToUnlock, fragmentIndexToUnlock);
			//GeneralFinder.informativeManager.

			informativeMan.c_UnlockFragment(sectionIndexToUnlock, fragmentIndexToUnlock);

			disableThisObject();
			
		}
		
		
	}

	void disableThisObject() {

		fragmentUnlocked = true;
		
		SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
		
		sr.enabled = false;

	}
	
	public void OnTriggerEnter2D(Collider2D c) {
		
		if (c.tag == "Player")
			getFragment ();
		
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
	
	public void c_setFragmentInt(int frag) {
		
		fragmentIndexToUnlock = frag;
		
	}
	
}
