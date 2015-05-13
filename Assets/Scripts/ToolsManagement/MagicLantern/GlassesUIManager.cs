using UnityEngine;
using System.Collections;

public class GlassesUIManager : MonoBehaviour {

	GlassesManager glassesManager;
	
	public Sprite KeyboardSprite;
	public Sprite ControllerSprite;

	GameObject controller;
	CursorHandler cursorHandler;

	GameObject canvasPlayingUI;
	PlayingUI playingUI;

	Glass actualGlass;
	bool actualUseController;
	bool actualActive;

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

		if (canvasPlayingUI != null && magicLanternLogic.active) {
			updateUI ();
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
				}
				
				if (actualUseController != cursorHandler.useController) {
					actualUseController = cursorHandler.useController;
					updateButtonUI();
				}
				
				if (actualActive != magicLanternLogic.active)
				{
					actualActive = magicLanternLogic.active;
					updateUI();
					updateButtonUI();
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

	void updateUI()
	{
		Sprite[] tempSpriteList = new Sprite[1];
		tempSpriteList[0] = actualGlass.glassSprite;
		
		playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomRight);

		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);
	}

	void updateButtonUI()
	{
		if (actualUseController)
			playingUI.setButtonSprite (PlayingUI.UIPosition.BottomRight, ControllerSprite);
		else
			playingUI.setButtonSprite (PlayingUI.UIPosition.BottomRight, KeyboardSprite);
		
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);
	}

	void deleteUI()
	{
		Sprite[] tempSpriteList = new Sprite[1];
		tempSpriteList [0] = new Sprite ();
		playingUI.setSprites (tempSpriteList, PlayingUI.UIPosition.BottomRight);
		playingUI.setButtonSprite (PlayingUI.UIPosition.BottomRight, tempSpriteList [0]);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.BottomRight);
	}
}
