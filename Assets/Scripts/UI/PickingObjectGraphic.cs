using UnityEngine;
using System.Collections;

public class PickingObjectGraphic : MonoBehaviour {

	CameraHandler cameraHandler;
	GameObject canvasPlayingUI;
	PlayingUI playingUI;
	PlayingUILateral playingUILateral;

	GameObject gameObjectUI;
	UnityEngine.UI.Image imageComponent;
	RectTransform rectTransform;

	Sprite sprite;

	Vector2 bigScale;

	Vector2 destinationPosition;
	Vector2 destinationSize;
	PlayingUI.UIPosition posCorner;
	PlayingUILateral.UIPosition posLateral;
	bool lateral = false;
	int index;

	bool gettingBig = true;
	bool gettingSmall = false;
	float beginningTime = 0.0f;

	float timeToGetSmall = 3.0f;

	float lerpSpeed = 1.0f;

	public float canvasXSize;
	public float canvasYSize;

	void Start () {
		cameraHandler = Camera.main.gameObject.GetComponent<CameraHandler>();

		canvasPlayingUI = GameObject.FindGameObjectWithTag("CanvasPlayingUI");
		if (canvasPlayingUI != null)
		{
			playingUI = canvasPlayingUI.GetComponent<PlayingUI>();
			playingUILateral = canvasPlayingUI.GetComponent<PlayingUILateral>();
			canvasXSize = canvasPlayingUI.GetComponent<RectTransform>().sizeDelta.x;
			canvasYSize = canvasPlayingUI.GetComponent<RectTransform>().sizeDelta.y;
		}

		createComponents();
		setStartingPositionScale();

		beginningTime = Time.time;

		setDestinationPosition();

		Color colorTemp = imageComponent.color;
		imageComponent.color = new Color(colorTemp.r, colorTemp.g, colorTemp.b, 0.0f);
	}

	//crea l'oggetto UI, i suoi component e ne salva i riferimenti
	void createComponents()
	{
		gameObjectUI = new GameObject();
		gameObjectUI.transform.parent = canvasPlayingUI.transform;
		gameObjectUI.name = "PickedObject";
		imageComponent = gameObjectUI.AddComponent<UnityEngine.UI.Image>();
		rectTransform = gameObjectUI.GetComponent<RectTransform>();
		imageComponent.preserveAspect = true;
		rectTransform.localScale = new Vector3(1.0f,1.0f,1.0f);
	}

	//setta le variabili da chiamare alla creazione del component, due metodi diversi
	public void setVariables(Sprite sprite, PlayingUI.UIPosition pos, int index, float scale = 400.0f, float lerpSpeed = 4.0f, float timeToGetSmall = 3.0f)
	{
		this.sprite = sprite;
		this.bigScale = new Vector2(scale, scale);
		this.lerpSpeed = lerpSpeed;
		this.posCorner = pos;
		this.index = index;
		this.timeToGetSmall = timeToGetSmall;
		this.lateral = false;
	}

	public void setVariables(Sprite sprite, PlayingUILateral.UIPosition pos, int index, float scale = 400.0f, float lerpSpeed = 4.0f, float timeToGetSmall = 3.0f)
	{
		this.sprite = sprite;
		this.bigScale = new Vector2(scale, scale);
		this.lerpSpeed = lerpSpeed;
		this.posLateral = pos;
		this.index = index;
		this.timeToGetSmall = timeToGetSmall;
		this.lateral = true;
	}

	//setta la posizione iniziale e la scala
	void setStartingPositionScale()
	{
		imageComponent.sprite = sprite;
		rectTransform.anchorMin = new Vector2(0.5f,0.5f);
		rectTransform.anchorMax = new Vector2(0.5f,0.5f);
		rectTransform.pivot = new Vector2(0.5f,0.5f);
		rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
		rectTransform.sizeDelta = new Vector2 (0.0f, 0.0f);
	}

	//setto la posizione finale, cambia a seconda dell'oggetto passato come input
	void setDestinationPosition()
	{
		//bisogna raggiungere una posizione in un angolo dello schermo
		if (!lateral && posCorner != null)
		{
			float yPadding = playingUI.getYPadding();
			float xPadding = playingUI.getXPadding();
			destinationSize = new Vector2(playingUI.getSpritesGroupSize(posCorner), playingUI.getSpritesGroupSize(posCorner));
			if (posCorner == PlayingUI.UIPosition.BottomRight)
			{
				float XPosition = canvasXSize/2 - xPadding - destinationSize.x/2;
				float YPosition = -canvasYSize/2 + yPadding + destinationSize.y/2;
				destinationPosition = new Vector2(XPosition, YPosition);
			}
			else if (posCorner == PlayingUI.UIPosition.BottomLeft)
			{
				float XPosition = -canvasXSize/2 + xPadding + destinationSize.x/2;
				float YPosition = -canvasYSize/2 + yPadding + destinationSize.y/2;
				destinationPosition = new Vector2(XPosition, YPosition);
			}
			else if (posCorner == PlayingUI.UIPosition.UpperLeft)
			{
				float XPosition = -canvasXSize/2 + xPadding + destinationSize.x/2;
				float YPosition = canvasYSize/2 - yPadding - destinationSize.y/2;
				destinationPosition = new Vector2(XPosition, YPosition);
			}
			else if (posCorner == PlayingUI.UIPosition.UpperRight)
			{
				float XPosition = canvasXSize/2 - xPadding - destinationSize.x/2;
				float YPosition = canvasYSize/2 - yPadding - destinationSize.y/2;
				destinationPosition = new Vector2(XPosition, YPosition);
			}
		}
		//bisogna raggiungere una posizione laterale
		else if (lateral && posLateral != null)
		{
			//Debug.Log ("lateral");
			destinationSize = new Vector2(playingUILateral.getSpritesGroupSize(posLateral), playingUILateral.getSpritesGroupSize(posLateral));
			int xPadding = playingUILateral.getXWhenVisible();
			GameObject tempGO = playingUILateral.getImageObject(posLateral, index);
			if (tempGO != null)
			{
				//Debug.Log ("qui");
				RectTransform tempRectTransform = tempGO.GetComponent<RectTransform>();
				if (tempRectTransform != null)
				{
					float yPadding = tempRectTransform.anchoredPosition.y;
					if (posLateral == PlayingUILateral.UIPosition.Right)
					{
						//Debug.Log ("quiRight");
						float XPosition = canvasXSize/2 - xPadding;
						float YPosition = yPadding;
						destinationPosition = new Vector2(XPosition, YPosition);
					}
					else if (posLateral == PlayingUILateral.UIPosition.Left)
					{
						//Debug.Log ("quiLeft");
						float XPosition = -canvasXSize/2 + xPadding;
						float YPosition = yPadding;
						destinationPosition = new Vector2(XPosition, YPosition);
					}
				}
			}
		}
	}
	
	void Update () {
		//transform.position = cameraHandler.getCameraPositionZEnvironment();
		if (gettingBig)
		{
			Color colorTemp = imageComponent.color;
			float alpha = Mathf.Lerp(colorTemp.a, 1.0f, Time.deltaTime * lerpSpeed);
			imageComponent.color = new Color(colorTemp.r, colorTemp.g, colorTemp.b, alpha);
			
			//Vector2 sizeTemp = rectTransform.sizeDelta;
			Vector2 sizeTemp = Vector2.Lerp(rectTransform.sizeDelta, bigScale,  Time.deltaTime * lerpSpeed);
			rectTransform.sizeDelta = sizeTemp;

			if ((Time.time - beginningTime) > timeToGetSmall)
			{
				gettingBig = false;
				//beginningTime = Time.time;
				gettingSmall = true;
			}
		}

		if (gettingSmall)
		{
			Vector2 actualPosition = rectTransform.anchoredPosition;
			actualPosition = Vector2.Lerp(actualPosition, destinationPosition, Time.deltaTime * lerpSpeed);
			rectTransform.anchoredPosition = actualPosition;

			Color colorTemp = imageComponent.color;
			float alpha = Mathf.Lerp(colorTemp.a, 0.0f, Time.deltaTime * lerpSpeed);
			imageComponent.color = new Color(colorTemp.r, colorTemp.g, colorTemp.b, alpha);

			//Vector2 sizeTemp = rectTransform.sizeDelta;
			Vector2 sizeTemp = Vector2.Lerp(rectTransform.sizeDelta, destinationSize,  Time.deltaTime*  lerpSpeed);
			rectTransform.sizeDelta = sizeTemp;
		}
	}
}
