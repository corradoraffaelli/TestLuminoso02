using UnityEngine;
using System.Collections;

public class PlayingUITests : MonoBehaviour {

	PlayingUI playingUI;
	PlayingUILateral playingUILateral;

	public Sprite[] right01;
	public Sprite[] right02;
	public Sprite[] left01;
	public Sprite[] left02;

	bool left = true;
	bool right = true;
	
	void Start () {
		playingUI = GetComponent<PlayingUI>();
		playingUILateral = GetComponent<PlayingUILateral>();
	}

	void Update () {
		if (Input.GetKeyUp(KeyCode.N))
			playingUILateral.showIcons(PlayingUILateral.UIPosition.Right);

		if (Input.GetKeyUp(KeyCode.M))
			playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, false);

		if (Input.GetKeyUp(KeyCode.V))
			playingUILateral.showIcons(PlayingUILateral.UIPosition.Left);
		
		if (Input.GetKeyUp(KeyCode.B))
			playingUILateral.showIcons(PlayingUILateral.UIPosition.Left, false);

		if (Input.GetKeyUp(KeyCode.K))
		{
			if (left)
				playingUILateral.setSprites(left02, PlayingUILateral.UIPosition.Left);
			else
				playingUILateral.setSprites(left01, PlayingUILateral.UIPosition.Left);

			left = !left;
			playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Left);
		}

		if (Input.GetKeyUp(KeyCode.L))
		{
			if (right)
				playingUILateral.setSprites(right02, PlayingUILateral.UIPosition.Right);
			else
				playingUILateral.setSprites(right01, PlayingUILateral.UIPosition.Right);

			right = !right;
			playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Right);
		}

		if (Input.GetKeyUp(KeyCode.J))
		{
			playingUILateral.setExclamation(PlayingUILateral.UIPosition.Right, 2, true);
			playingUILateral.setExclamation(PlayingUILateral.UIPosition.Right, 3, true);
		}
		if (Input.GetKeyUp(KeyCode.H))
		{
			playingUILateral.setExclamation(PlayingUILateral.UIPosition.Right, 2, false);
		}
		if (Input.GetKeyUp(KeyCode.G))
		{
			playingUILateral.removeAllExclamations();
		}

		if (Input.GetKeyUp(KeyCode.O))
		{
			PulsingUIElement pulsing = gameObject.AddComponent<PulsingUIElement>();
			pulsing.setVariables(playingUILateral.getImageObject(PlayingUILateral.UIPosition.Right, 1),10.0f, 1.5f, 4.0f);
			PulsingUIElement pulsing02 = gameObject.AddComponent<PulsingUIElement>();
			pulsing02.setVariables(playingUI.getImageObject(PlayingUI.UIPosition.UpperRight, 0),10.0f, 1.5f, 4.0f);
		}
			
	}
}
