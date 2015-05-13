using UnityEngine;
using System.Collections;

public class GlassPiecesUIManager : MonoBehaviour {

	[System.Serializable]
	public class GlassPiecesImages
	{
		public Sprite takenSprite;
		public Sprite notTakenSprite;
		public bool used = false;
	}

	[System.Serializable]
	class GlassElement
	{
		public GlassPiecesImages[] glassPiecesImages;
		public Glass glass;
	}

	[SerializeField]
	GlassPiecesImages[] piecesImages = new GlassPiecesImages[10];

	[SerializeField]
	GlassElement[] glassElement;

	MagicLantern magicLanternLogic;
	GlassesManager glassesManager;

	GameObject canvasPlayingUI;
	PlayingUI playingUI;

	//lista dei component subGlass
	public subGlass[] subGlassList;
	//array di bool che indicano se, nell'update precedente il frammento è stato raccolto o meno, serve per verificare che ci siano aggiornamenti
	public bool[] oldSubGlassListTaken;

	public int subGlassesNumber = 0;

	// Use this for initialization
	void Start () {
		magicLanternLogic = GetComponent<MagicLantern> ();
		glassesManager = GetComponent<GlassesManager> ();

		canvasPlayingUI = GameObject.FindGameObjectWithTag ("CanvasPlayingUI");
		if (canvasPlayingUI == null)
			Debug.Log ("ATTENZIONE!! canvasPlayingUI non trovato!! Assicurarsi che il relativo prefab sia nella scena");
		else
			playingUI = canvasPlayingUI.GetComponent<PlayingUI> ();

		//randomNum = Random.Range (0, 9);

		fillGlassElements ();

		createOldBoolList ();

		setUI ();

		updateSubGlassList ();
	}
	
	// Update is called once per frame
	void Update () {

		if (verifyIfChanged () == true) {
			setUI();
		}
	}

	void fillGlassElements()
	{
		subGlassesNumber = 0;

		int glassesNum = 0;
		Glass[] glasses = glassesManager.getGlassList ();
		for (int i = 0; i< glasses.Length; i++) {
			if (glasses[i].subGlassList.Length != 0)
				glassesNum ++;
		}

		glassElement = new GlassElement[glassesNum];

		glassesNum = 0;
		for (int i = 0; i< glasses.Length; i++) {
			if (glasses[i].subGlassList.Length != 0)
			{
				//creoo l'oggetto che contiene il vetrino
				glassElement[glassesNum] = new GlassElement();

				//prendo il riferimento al vetrino
				glassElement[glassesNum].glass = glasses[i];

				//creo gli elementi che contengono le immagini dei frammenti di vetrino
				glassElement[glassesNum].glassPiecesImages = new GlassPiecesImages[glasses[i].subGlassList.Length];

				for (int j = 0; j < glassElement[glassesNum].glassPiecesImages.Length; j++)
				{
					subGlassesNumber++;
					glassElement[glassesNum].glassPiecesImages[j] = giveRandomPiecesImages();
				}

				glassesNum ++;
			}
				
		}
	}

	GlassPiecesImages giveRandomPiecesImages()
	{
		while (true) {
			int randomNum = Random.Range(0,piecesImages.Length);
			if (!piecesImages[randomNum].used)
			{
				piecesImages[randomNum].used = true;
				return piecesImages[randomNum];
			}
		}

	}

	void setUI()
	{
		Sprite[] tempSprites = new Sprite[subGlassesNumber];
		int tempSpriteNum = 0;

		for (int i = 0; i< glassElement.Length; i++) {

			int spriteElementNum = 0;

			for (int j=0; j< glassElement[i].glass.subGlassList.Length; j++)
			{
				subGlass sottoVetrino = glassElement[i].glass.subGlassList[j].GetComponent<subGlass>();
				if (sottoVetrino.taken)
				{
					tempSprites[tempSpriteNum] = glassElement[i].glassPiecesImages[spriteElementNum].takenSprite;
				}else{
					tempSprites[tempSpriteNum] = glassElement[i].glassPiecesImages[spriteElementNum].notTakenSprite;
				}
					
				spriteElementNum++;
				tempSpriteNum++;
			}
		}

		playingUI.setSprites (tempSprites, PlayingUI.UIPosition.UpperLeft);
		playingUI.setVertical (PlayingUI.UIPosition.UpperLeft, true);
		playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperLeft);
	}

	void updateSubGlassList()
	{
		subGlassList = new subGlass[subGlassesNumber];
		int subGlassListNum = 0;

		for (int i = 0; i< glassElement.Length; i++) {
			
			for (int j=0; j< glassElement[i].glass.subGlassList.Length; j++)
			{
				subGlassList[subGlassListNum] = glassElement[i].glass.subGlassList[j].GetComponent<subGlass>();
				subGlassListNum ++;
			}
		}

	}

	//metodo che controllo se è stato raccolto un qualche vetrino
	bool verifyIfChanged()
	{
		if (oldSubGlassListTaken != null) {
			for (int i = 0; i<subGlassList.Length; i++)
			{
				if (oldSubGlassListTaken[i] != subGlassList[i].taken)
				{
					oldSubGlassListTaken[i] = subGlassList[i].taken;
					//Debug.Log ("cambiato");
					return true;
				}
			}
		}
		return false;
	}

	void createOldBoolList()
	{
		oldSubGlassListTaken = new bool[subGlassesNumber];
		for (int i = 0; i<subGlassList.Length; i++)
		{
			oldSubGlassListTaken[i] = subGlassList[i].taken;
		}

	}
}
