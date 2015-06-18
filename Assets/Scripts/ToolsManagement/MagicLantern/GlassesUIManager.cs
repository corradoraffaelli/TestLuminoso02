using UnityEngine;
using System.Collections;

public class GlassesUIManager : MonoBehaviour {

	GlassesManager glassesManager;
	
	public Sprite keyboardSprite;
	public Sprite controllerSprite;

	public Sprite modify;
	public Sprite modKeyboardSprite;
	public Sprite modControllerSprite;

	GameObject controller;
	CursorHandler cursorHandler;

	GameObject canvasPlayingUI;
	PlayingUI playingUI;

	Glass actualGlass;
	bool actualUseController;
	bool actualActive;

	int usableGlassesNumber;

	MagicLantern magicLanternLogic;

	void Start () {
		canvasPlayingUI = GameObject.FindGameObjectWithTag ("CanvasPlayingUI");
		if (canvasPlayingUI == null)
			Debug.Log ("ATTENZIONE!! canvasPlayingUI non trovato!! Assicurarsi che il relativo prefab sia nella scena");
		else
			playingUI = canvasPlayingUI.GetComponent<PlayingUI> ();

		glassesManager = GetComponent<GlassesManager> ();

		controller = GameObject.FindGameObjectWithTag ("Controller");
		cursorHandler = controller.GetComponent<CursorHandler> ();

		magicLanternLogic = GetComponent<MagicLantern> ();

		actualGlass = glassesManager.getActualGlass ();
		actualUseController = cursorHandler.useController;
		usableGlassesNumber = glassesManager.getUsableGlassList().Length;

		if (canvasPlayingUI != null && magicLanternLogic.active) {
			updateUI ();
			if (usableGlassesNumber > 1)
				updateButtonUI ();
		}

	}

	void Update () {

		if (canvasPlayingUI != null) {
			if (magicLanternLogic.active)
			{


				if (actualGlass != glassesManager.getActualGlass ()) {
					actualGlass = glassesManager.getActualGlass ();
					updateUI();
					updateModifyUI();
				}

				if (actualUseController != cursorHandler.useController || usableGlassesNumber != glassesManager.getUsableGlassList().Length) {
					usableGlassesNumber = glassesManager.getUsableGlassList().Length;
					actualUseController = cursorHandler.useController;
					updateButtonUI();
					updateModifyUI();
				}
				
				if (actualActive != magicLanternLogic.active)
				{
					actualActive = magicLanternLogic.active;
					updateUI();
					updateButtonUI();
					updateModifyUI();
				}
			}else{
				if (actualActive != magicLanternLogic.active)
				{
					actualActive = magicLanternLogic.active;
					deleteUI();
				}
			}
		}
	}

	public void updateUI()
	{
		Sprite[] tempSpriteList = new Sprite[1];
		tempSpriteList[0] = actualGlass.glassSprite;
		
		playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomRight);

		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);
	}

	public void removeButtonUI()
	{
		playingUI.cleanPositionButtonObject(PlayingUI.UIPosition.BottomRight);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);
	}

	public void updateButtonUI()
	{
		if (glassesManager.getUsableGlassList().Length > 1)
		{
			if (actualUseController)
				playingUI.setButtonSprite (PlayingUI.UIPosition.BottomRight, controllerSprite);
			else
				playingUI.setButtonSprite (PlayingUI.UIPosition.BottomRight, keyboardSprite);
		}
		else
		{
			playingUI.cleanPositionButtonObject(PlayingUI.UIPosition.BottomRight);
		}

		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);

	}

	public void updateModifyUI()
	{
		if (actualGlass.canBeModified) {
			Sprite[] tempSpriteList = new Sprite[1];
			tempSpriteList [0] = modify;

			playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomLeft);
			if (actualUseController)
				playingUI.setButtonSprite (PlayingUI.UIPosition.BottomLeft, modControllerSprite);
			else
				playingUI.setButtonSprite (PlayingUI.UIPosition.BottomLeft, modKeyboardSprite);

			playingUI.setSpritesSize(PlayingUI.UIPosition.BottomLeft, PlayingUI.UISize.Big);
			playingUI.setVerticalButton(PlayingUI.UIPosition.BottomLeft, false);

		} else {
			/*
			Sprite[] tempSpriteList = new Sprite[1];
			tempSpriteList [0] = new Sprite ();

			playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomLeft);
			playingUI.setButtonSprite (PlayingUI.UIPosition.BottomLeft, tempSpriteList [0]);
			playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomLeft);
			*/
			playingUI.cleanPositionGameObjects (PlayingUI.UIPosition.BottomLeft);
			playingUI.cleanPositionButtonObject (PlayingUI.UIPosition.BottomLeft);
			playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomLeft);
		}

		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomLeft);
			
	}

	void deleteUI()
	{
		playingUI.cleanPositionGameObjects (PlayingUI.UIPosition.BottomLeft);
		playingUI.cleanPositionButtonObject (PlayingUI.UIPosition.BottomLeft);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomLeft);

		playingUI.cleanPositionGameObjects (PlayingUI.UIPosition.BottomRight);
		playingUI.cleanPositionButtonObject (PlayingUI.UIPosition.BottomRight);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);
		/*
		Sprite[] tempSpriteList = new Sprite[1];
		tempSpriteList [0] = new Sprite ();

		playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomRight);
		playingUI.setButtonSprite (PlayingUI.UIPosition.BottomRight, tempSpriteList [0]);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);

		playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomLeft);
		playingUI.setButtonSprite (PlayingUI.UIPosition.BottomLeft, tempSpriteList [0]);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomLeft);
		*/
	}
	
}
