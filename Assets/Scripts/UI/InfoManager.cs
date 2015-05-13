using UnityEngine;
using System.Collections;

public class InfoManager : MonoBehaviour {

	public Sprite info;
	public Sprite noInfo;
	public Sprite controllerButton;
	public Sprite keyboardButton;

	GameObject controller;
	CursorHandler cursorHandler;

	GameObject canvasPlayingUI;
	PlayingUI playingUI;
	
	void Start () {
		canvasPlayingUI = GameObject.FindGameObjectWithTag ("CanvasPlayingUI");
		if (canvasPlayingUI == null)
			Debug.Log ("ATTENZIONE!! canvasPlayingUI non trovato!! Assicurarsi che il relativo prefab sia nella scena");
		else
			playingUI = canvasPlayingUI.GetComponent<PlayingUI> ();

		controller = GameObject.FindGameObjectWithTag ("Controller");
		cursorHandler = controller.GetComponent<CursorHandler> ();

		Sprite[] sprites = new Sprite[1];
		sprites [0] = info;



		playingUI.setSprites (sprites, PlayingUI.UIPosition.UpperRight);


	
	}
	
	// Update is called once per frame
	void Update () {

		Sprite buttonSprite;

		if (cursorHandler.useController)
			buttonSprite = controllerButton;
		else
			buttonSprite = keyboardButton;

		playingUI.setButtonSprite (PlayingUI.UIPosition.UpperRight, buttonSprite);

		playingUI.setVerticalButton (PlayingUI.UIPosition.UpperRight, false);
		playingUI.setSpritesSize (PlayingUI.UIPosition.UpperRight, PlayingUI.UISize.Big);

		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);

	}
}
