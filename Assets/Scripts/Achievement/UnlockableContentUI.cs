using UnityEngine;
using System.Collections;

public class UnlockableContentUI : MonoBehaviour {

	[System.Serializable]
	public class SpritesBook{
		public Sprite normalBook;
		public Sprite exclamationBook;
		public Sprite keyboardButton;
		public Sprite controllerButton;
		public Sprite pageSprite;
	}

	[SerializeField]
	SpritesBook spritesBook;

	public bool saveOnDestroy = true;

	public float timeToHideLateral = 4.0f;
	public float timeToPulse = 4.0f;

	public float timeToGetSmall = 1.5f;

	int changeSpriteTime = 0;
	int maxChangeSpriteTime = 2;
	bool beginChanged = false;
	
	Sprite[] rightSprites;
	Sprite[] leftSprites;

	public InformativeSection actualContentSection;
	InformativeSection actualFactSection;
	InformativeSection actualFragmentSection;

	bool showRight = false;
	float showRightTime;
	bool showLeft = false;
	float showLeftTime;

	PulsingUIElement pulsingBook;

	bool wasPulsingBookNull = false;

	void Start () {
		setSections();
		
		setUpperRightVariables();
		setUpperRightStandardBook();
		setUpperRightButton();
	}

	void Update () {
		//non dovrebbe esistere, dopo qualche update aggiorna le sprites iniziali
		handleDelayInit();

		//nasconde le barre laterali dopo tot secondi, nel caso siano visibili
		hideLateralManager();

		if (!wasPulsingBookNull && pulsingBook == null)
		{
			setUpperRightStandardBook();
			wasPulsingBookNull = true;
		}
	}

	void setSections()
	{
		///*
		actualContentSection = GeneralFinder.informativeManager.getActualCollectiblesSection(GeneralFinder.informativeManager.actualLevelNumber);
		actualFactSection = GeneralFinder.informativeManager.getActualFunFactsSection(GeneralFinder.informativeManager.actualLevelNumber);
		actualFragmentSection = GeneralFinder.informativeManager.getActualFragmentSection(GeneralFinder.informativeManager.actualLevelNumber);
		//*/
		/*
		actualContentSection = GeneralFinder.informativeManager.getActualCollectiblesSection();
		actualFactSection = GeneralFinder.informativeManager.getActualFunFactsSection();
		actualFragmentSection = GeneralFinder.informativeManager.getActualFragmentSection();
		*/
		/*
		actualContentSection = GeneralFinder.informativeManager.getActualCollectiblesSection(1);
		actualFactSection = GeneralFinder.informativeManager.getActualFunFactsSection(1);
		actualFragmentSection = GeneralFinder.informativeManager.getActualFragmentSection(1);
		*/
	}

	//questo metodo non dovrebbe esistere, ma non capisco perché, l'update delle sprites allo start non funziona (classi Dario??)
	//perciò lo faccio dopo qualche update
	void handleDelayInit()
	{
		//prende le sezioni di default del livello attuale


		if (!beginChanged)
		{
			if (changeSpriteTime < maxChangeSpriteTime)
				changeSpriteTime++;
			else
			{
				updateContentSprites();
				updateFragmentSprites();
				beginChanged = true;
				//Debug.Log ("aggiornate all'avvio le sprites");
			}
		}
	}

	public void unlockFragment(string id)
	{
		Debug.Log ("ho sbloccato il frammento "+id);

		//se è un frammento devo
		//1. aggiornare l'array di sprites a sinistra
		//2. mostrare le sprites a sinistra aggiornate
		//3. eseguire l'animazione dell'oggetto raccolto

		//1.
		updateFragmentSprites();
		
		//2.
		GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Left, true);
		showLeft = true;
		showLeftTime = Time.time;

		//3.
		PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
		pick.setVariables(findContent(actualFragmentSection, id).iconUnlock, PlayingUILateral.UIPosition.Left, findContentIndex(actualFragmentSection, id));
		pick.setTimeToGetSmall(timeToGetSmall);

	}

	public void unlockContent(string name)
	{
		Debug.Log ("ho sbloccato l'oggetto "+name);

		//se è un collezionabile devo
		//1. aggiornare l'array di sprites a destra
		//2. mostrare le sprites a destra aggiornate
		//3. eseguire l'animazione dell'oggetto raccolto
		//4. mostrare l'icona del libro lampeggiante per tot secondi


		//1.
		updateContentSprites();

		//2.
		GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, true);
		showRight = true;
		showRightTime = Time.time;

		//3.
		PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
		pick.setVariables(findContent(actualContentSection, name).iconUnlock, PlayingUILateral.UIPosition.Right, findContentIndex(actualContentSection, name));
		pick.setTimeToGetSmall(timeToGetSmall);

		setUpperRightExclamationBook();
		setPulsingBook();

	}

	public void unlockFact(string name)
	{
		Debug.Log ("ho sbloccato il fun fact "+name);

		//3.
		PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
		pick.setVariables(spritesBook.pageSprite, PlayingUI.UIPosition.UpperRight,0);
		pick.setBookPage(true);
		//pick.setTimeToGetSmall(timeToGetSmall);

		setUpperRightExclamationBook();
		setPulsingBook();
	}

	void updateContentSprites()
	{
		rightSprites = new Sprite[actualContentSection.contents.Length];
		int actualIndex = 0;
		for (int j = 0; j < actualContentSection.contents.Length; j++)
		{
			if (actualContentSection.contents[j] != null && actualContentSection.contents[j].unlockerObject != null
			    && actualContentSection.contents[j].iconLock != null && actualContentSection.contents[j].iconUnlock != null)
			{
				if (actualContentSection.contents[j].locked)
					rightSprites[actualIndex] = actualContentSection.contents[j].iconLock;
				else
					rightSprites[actualIndex] = actualContentSection.contents[j].iconUnlock;
				
				actualIndex++;
			}
		}
		GeneralFinder.playingUILateral.setSprites(rightSprites, PlayingUILateral.UIPosition.Right);
		GeneralFinder.playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Right);
	}

	void updateFragmentSprites()
	{
		leftSprites = new Sprite[actualFragmentSection.contents.Length];
		int actualIndex = 0;
		for (int j = 0; j < actualFragmentSection.contents.Length; j++)
		{
			if (actualFragmentSection.contents[j] != null && actualFragmentSection.contents[j].unlockerObject != null
			    && actualFragmentSection.contents[j].iconLock != null && actualFragmentSection.contents[j].iconUnlock != null)
			{
				if (actualFragmentSection.contents[j].locked)
					leftSprites[actualIndex] = actualFragmentSection.contents[j].iconLock;
				else
					leftSprites[actualIndex] = actualFragmentSection.contents[j].iconUnlock;
				
				actualIndex++;
			}
		}
		GeneralFinder.playingUILateral.setSprites(leftSprites, PlayingUILateral.UIPosition.Left);
		GeneralFinder.playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Left);
	}

	//nascone le barre laterali dopo un tot di tempo
	void hideLateralManager()
	{
		if (showRight)
		{
			if ((Time.time - showRightTime) > timeToHideLateral)
			{
				GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, false);
				showRight = false;
			}
		}

		if (showLeft)
		{
			if ((Time.time - showLeftTime) > timeToHideLateral)
			{
				GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Left, false);
				showLeft = false;
			}
		}
	}

	//ritorna l'attuale content sbloccato, a partire dal suo nome
	InformativeContent findContent(InformativeSection section, string name)
	{
		if (section != null && section.contents != null && section.contents.Length != 0)
		{
			for (int j = 0; j < section.contents.Length; j++)
			{
				if (section.contents[j].name == name)
					return section.contents[j];
			}
		}
		return null;
	}

	//trovo l'indice del content attuale rispetto al numero totale (senza fun facts)
	//funzione che non servirebbe se i fun facts fossero gestiti separatamente
	int findContentIndex(InformativeSection section, string name)
	{
		if (section != null && section.contents != null && section.contents.Length != 0)
		{
			for (int j = 0; j < section.contents.Length; j++)
			{
				if (section.contents[j].name == name)
					return j;
			}
		}
		return 0;
	}

	void setUpperRightVariables()
	{
		GeneralFinder.playingUI.setVerticalButton (PlayingUI.UIPosition.UpperRight, false);
		GeneralFinder.playingUI.setSpritesSize (PlayingUI.UIPosition.UpperRight, PlayingUI.UISize.Big);
	}

	void setUpperRightStandardBook()
	{
		Sprite[] sprites = new Sprite[1];
		sprites [0] = spritesBook.normalBook;

		if (GeneralFinder.playingUI != null) {
			GeneralFinder.playingUI.setSprites (sprites, PlayingUI.UIPosition.UpperRight);
			GeneralFinder.playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);
		}
	}
	void setPulsingBook()
	{
		pulsingBook = gameObject.AddComponent<PulsingUIElement>();
		//pulsingBook.setVariables(GeneralFinder.playingUI.getImageObject(PlayingUI.UIPosition.UpperRight, 0),8.0f, 1.1f, 1.0f);
		pulsingBook.setVariables(GeneralFinder.playingUI.getButtonObject(PlayingUI.UIPosition.UpperRight),8.0f, 1.1f, 1.0f);
		wasPulsingBookNull = false;
	}

	void setUpperRightExclamationBook()
	{
		Sprite[] sprites = new Sprite[1];
		sprites [0] = spritesBook.exclamationBook;
		
		if (GeneralFinder.playingUI != null) {
			GeneralFinder.playingUI.setSprites (sprites, PlayingUI.UIPosition.UpperRight);
			GeneralFinder.playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);
		}
	}

	void setUpperRightButton()
	{
		Sprite buttonSprite;

		if (GeneralFinder.cursorHandler.useController)
			buttonSprite = spritesBook.controllerButton;
		else
			buttonSprite = spritesBook.keyboardButton;

		if (GeneralFinder.playingUI != null) {
			GeneralFinder.playingUI.setButtonSprite (PlayingUI.UIPosition.UpperRight, buttonSprite);
			GeneralFinder.playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);
		}

	}

	void OnDestroy()
	{
		if (saveOnDestroy)
		{
			GeneralFinder.informativeManager.c_saveData();
			Debug.Log ("salvo");
		}
			
	}
}
