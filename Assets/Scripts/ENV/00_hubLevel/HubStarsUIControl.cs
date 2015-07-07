using UnityEngine;
using System.Collections;

public class HubStarsUIControl : MonoBehaviour {

	public Sprite starSprite;
	public int XDistance;
	public int textSize = 60;
	public Color textColor = Color.white;
	public Font textFont;
	public PlayingUI.UIPosition position;

	int starsNumber = 0;
	public GameObject starGameObject;

	int updateBeforeStart = 5;
	int actualUpdate = 0;
	bool settato = false;

	void Start () {
		HubFragmentCounter fragmentCounter = GetComponent<HubFragmentCounter>();
		if (fragmentCounter != null)
			starsNumber = fragmentCounter.fragmentTotalNumber;
		if (starsNumber != 0)
			setStarSprite();
	}

	void Update () {
		if (!settato && starsNumber != 0)
			setStarNumber();
	}

	void setStarSprite()
	{
		if (GeneralFinder.canvasPlayingUI != null && GeneralFinder.playingUI != null)
		{
			Sprite[] groupSprite = new Sprite[1];
			groupSprite[0] = starSprite;
			GeneralFinder.playingUI.setSprites(groupSprite, position);
			GeneralFinder.playingUI.updateSpritesOnScreen(position);
		}
	}

	void setStarNumber()
	{
		starGameObject = GeneralFinder.playingUI.getImageObject(position, 0);
		if (starGameObject != null)
		{
			//RectTransform rectStar = starGameObject.GetComponent<RectTransform>();

			GameObject textObject = new GameObject ();
			textObject.transform.SetParent(starGameObject.transform);

			UnityEngine.UI.Text textComp = textObject.AddComponent<UnityEngine.UI.Text>();
			textComp.alignment = TextAnchor.UpperRight;
			textComp.color = textColor;
			textComp.fontSize = textSize;
			textComp.fontStyle = FontStyle.Bold;
			textComp.text = starsNumber.ToString();
			textComp.font = textFont;

			RectTransform rectTransform = textObject.GetComponent<RectTransform>();
			rectTransform.pivot = new Vector2 (1.0f, 1.0f);
			rectTransform.anchorMin = new Vector2 (1.0f, 1.0f);
			rectTransform.anchorMax = new Vector2 (1.0f, 1.0f);
			rectTransform.anchoredPosition = new Vector2(XDistance, 0.0f);
			rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

			settato = true;
		}
	}


}
