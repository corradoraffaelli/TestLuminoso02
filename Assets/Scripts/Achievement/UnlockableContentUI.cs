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
		public ButtonController.PS3Button controllerButtonType;
		public ButtonKeyboardMouse.Button keyboardButtonType;
	}

	public bool overlaySpritesStart = true;

	[SerializeField]
	SpritesBook spritesBook;

	public bool saveOnDestroy = true;

	public float timeToHideLateral = 4.0f;
	public float timeToPulse = 4.0f;

	public float timeToGetSmall = 1.5f;
	public float pageTimeToGetSmall = 4.0f;

	public float pulseButtonScale = 1.8f;
	public float pulseButtonSpeed = 3.0f;

	int changeSpriteTime = 0;
	int maxChangeSpriteTime = 2;
	bool beginChanged = false;
	
	Sprite[] rightSprites;
	Sprite[] leftSprites;

	InformativeSection actualContentSection;
	InformativeSection actualFactSection;
	InformativeSection actualFragmentSection;

	bool showRight = false;
	float showRightTime;
	bool showLeft = false;
	float showLeftTime;

	PulsingUIElement pulsingBook;

	bool wasPulsingBookNull = false;

	bool wasUseController;

	public bool activeBook = false;

	AudioHandler audioHandler;

	PulsingInfoButton centralButton;

	void Start () {
		setSections();

		if (countUnlockedElement() == 0)
			activeBook = false;
		else
			activeBook = true;

		if (overlaySpritesStart) {
			spritesBook.controllerButton = GeneralFinder.inputManager.getControllerSprite(spritesBook.controllerButtonType);
			spritesBook.keyboardButton = GeneralFinder.inputManager.getKeyboardSprite(spritesBook.keyboardButtonType);
		}

		if (activeBook)
		{
			setUpperRightVariables();
			setUpperRightStandardBook();
			setUpperRightButton();
		}

		//prima le seguenti due funzioni erano nell'handleDelayInit, leggi la descrizione per la motivazione
		updateContentSprites();
		updateFragmentSprites();

		audioHandler = GetComponent<AudioHandler>();

		wasUseController = GeneralFinder.cursorHandler.useController;



		centralButton = GeneralFinder.canvasPlayingUI.transform.GetComponentInChildren<PulsingInfoButton> ();
	}

	void Update () {
		//non dovrebbe esistere, dopo qualche update aggiorna le sprites iniziali
		//handleDelayInit();

		//nasconde le barre laterali dopo tot secondi, nel caso siano visibili
		hideLateralManager();

		if (activeBook && !wasPulsingBookNull && pulsingBook == null)
		{
			setUpperRightStandardBook();
			wasPulsingBookNull = true;
		}

		if (wasUseController != GeneralFinder.cursorHandler.useController)
		{
			wasUseController = GeneralFinder.cursorHandler.useController;

			if (overlaySpritesStart) {
				spritesBook.controllerButton = GeneralFinder.inputManager.getControllerSprite(spritesBook.controllerButtonType);
			}

			setUpperRightButton();
		}

		/*
		if (Input.GetKeyUp(KeyCode.F))
			stopPulsing();
		*/
	}

	void setSections()
	{
		actualContentSection = GeneralFinder.informativeManager.getActualCollectiblesSection();
		actualFactSection = GeneralFinder.informativeManager.getActualFunFactsSection();
		actualFragmentSection = GeneralFinder.informativeManager.getActualFragmentSection();
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
		if (actualFragmentSection != null)
		{
			Debug.Log ("ho sbloccato il frammento "+id);
			
			//se è un frammento devo
			//1. aggiornare l'array di sprites a sinistra
			//2. mostrare le sprites a sinistra aggiornate
			//3. eseguire l'animazione dell'oggetto raccolto
			//4. eseguire il suono
			
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

			//4.
			if (audioHandler != null)
				audioHandler.playClipByName("UnlockStar");
		}
	}

	public void unlockContent(string name)
	{
		if (actualContentSection != null)
		{
			Debug.Log ("ho sbloccato l'oggetto "+name);
			
			//se è un collezionabile devo
			//0. mostrare la sprite del libro, se non presente
			//1. aggiornare l'array di sprites a destra
			//2. mostrare le sprites a destra aggiornate
			//3. eseguire l'animazione dell'oggetto raccolto
			//4. mostrare l'icona del libro lampeggiante per tot secondi
			//4. eseguire il suono

			//0.
			if (!activeBook)
			{
				activeBook = true;
				setUpperRightVariables();
				setUpperRightStandardBook();
				setUpperRightButton();
				//Debug.Log ("entrato");
			}

			//1.
			updateContentSprites();
			
			//2.
			GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, true);
			showRight = true;
			showRightTime = Time.time;
			
			//3.
			PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
			pick.setVariables(findContent(actualContentSection, name).iconUnlock, PlayingUILateral.UIPosition.Right, findContentIndex(actualContentSection, name));
			pick.setBigScale(200.0f);
			pick.setTimeToGetSmall(timeToGetSmall);

			//pagina
			GameObject testGO = new GameObject();
			testGO.transform.position = transform.position;
			PickingObjectGraphic pick02 = testGO.AddComponent<PickingObjectGraphic>();
			pick02.setVariables(spritesBook.pageSprite, PlayingUI.UIPosition.UpperRight,0);
			pick02.setBookPage(true);
			pick02.setTimeToGetSmall(pageTimeToGetSmall);

			startCentralButtonPulsing();

			setUpperRightExclamationBook();
			//setPulsingBook();

			//4.
			if (audioHandler != null)
				audioHandler.playClipByName("UnlockContent");
		}
	}

	public void unlockFact(string name)
	{
		if (actualFactSection != null)
		{
			Debug.Log ("ho sbloccato il fun fact "+name);
			
			//se è un fun fact devo
			//0. mostrare la sprite del libro, se non presente
			//1. eseguire l'animazione della pagina raccolta
			//2. mostrare l'icona del libro lampeggiante per tot secondi
			//3. eseguire il suono

			//0.
			if (!activeBook)
			{
				activeBook = true;
				setUpperRightVariables();
				setUpperRightStandardBook();
				setUpperRightButton();
			}

			//1.
			PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
			pick.setVariables(spritesBook.pageSprite, PlayingUI.UIPosition.UpperRight,0);
			pick.setBookPage(true);
			pick.setTimeToGetSmall(pageTimeToGetSmall);
			
			//2.
			setUpperRightExclamationBook();
			//setPulsingBook();

			//3.
			if (audioHandler != null)
				audioHandler.playClipByName("UnlockFact");

			startCentralButtonPulsing();
		}
	}

	void updateContentSprites()
	{
		if (actualContentSection != null)
		{
			rightSprites = new Sprite[actualContentSection.contents.Length];
			int actualIndex = 0;
			for (int j = 0; j < actualContentSection.contents.Length; j++)
			{
				if (actualContentSection.contents[j] != null && actualContentSection.contents[j].unlockerObject != null
				    && actualContentSection.contents[j].iconLock != null && actualContentSection.contents[j].iconUnlock != null)
				{
					if (actualContentSection.contents[j].lockedContent)
						rightSprites[actualIndex] = actualContentSection.contents[j].iconLock;
					else
						rightSprites[actualIndex] = actualContentSection.contents[j].iconUnlock;
					
					actualIndex++;
				}
			}
			GeneralFinder.playingUILateral.setSprites(rightSprites, PlayingUILateral.UIPosition.Right);
			GeneralFinder.playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Right);
		}

	}

	void updateFragmentSprites()
	{
		if (actualFragmentSection != null)
		{
			leftSprites = new Sprite[actualFragmentSection.contents.Length];
			int actualIndex = 0;
			for (int j = 0; j < actualFragmentSection.contents.Length; j++)
			{
				if (actualFragmentSection.contents[j] != null && actualFragmentSection.contents[j].unlockerObject != null
				    && actualFragmentSection.contents[j].iconLock != null && actualFragmentSection.contents[j].iconUnlock != null)
				{
					if (actualFragmentSection.contents[j].lockedContent)
						leftSprites[actualIndex] = actualFragmentSection.contents[j].iconLock;
					else
						leftSprites[actualIndex] = actualFragmentSection.contents[j].iconUnlock;
					
					actualIndex++;
				}
			}
			GeneralFinder.playingUILateral.setSprites(leftSprites, PlayingUILateral.UIPosition.Left);
			GeneralFinder.playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Left);
		}
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
		pulsingBook.setVariables(GeneralFinder.playingUI.getButtonObject(PlayingUI.UIPosition.UpperRight),8.0f, pulseButtonScale, pulseButtonSpeed);
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

		if (overlaySpritesStart) {
			spritesBook.controllerButton = GeneralFinder.inputManager.getControllerSprite(spritesBook.controllerButtonType);
		}

		if (GeneralFinder.cursorHandler.useController)
			buttonSprite = spritesBook.controllerButton;
		else
			buttonSprite = spritesBook.keyboardButton;

		if (GeneralFinder.playingUI != null && activeBook) {
			GeneralFinder.playingUI.setButtonSprite (PlayingUI.UIPosition.UpperRight, buttonSprite);
			GeneralFinder.playingUI.updateSpritesOnScreen (PlayingUI.UIPosition.UpperRight);
		}
	}

	void OnDestroy()
	{
		/*
		if (saveOnDestroy)
		{
			GeneralFinder.informativeManager.c_saveData();
			Debug.Log ("salvo");
		}
		*/	
	}

	public void stopPulsing()
	{
		setUpperRightStandardBook();
		PulsingUIElement[] pulsingEl = GetComponents<PulsingUIElement>();
		for (int i = 0; i < pulsingEl.Length; i++)
		{
			if (pulsingEl[i] != null)
			{
				pulsingEl[i].stopPulsing();
			}
		}
	}

	int countUnlockedElement()
	{
		int unlockedNum = 0;

		if (actualFactSection != null)
		{
			for (int i = 0; i < actualFactSection.contents.Length; i++)
			{
				if (actualFactSection.contents[i] != null && !actualFactSection.contents[i].lockedContent)
					unlockedNum++;
			}
		}

		if (actualContentSection != null)
		{
			for (int i = 0; i < actualContentSection.contents.Length; i++)
			{
				if (actualContentSection.contents[i] != null && !actualContentSection.contents[i].lockedContent)
					unlockedNum++;
			}
		}

		return unlockedNum;
	}

	void startCentralButtonPulsing()
	{
		if (centralButton != null) {
			centralButton.changeSprite(GeneralFinder.inputManager.getSprite("OpenInformative"));
			centralButton.changeSpeed(1.84f);
			centralButton.changeMaxScale(1.2f);
			centralButton.changePulsingTimes(10);
			centralButton.active(true);
			centralButton.gameObject.transform.SetAsLastSibling();
		}
	}
}
