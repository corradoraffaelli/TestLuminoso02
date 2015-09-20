using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour {

	//InformativeManager informativeMan;

	//public GameObject canvasMenu;

	GameObject canvasIntro;
	GameObject canvasInformative;

	GameObject canvasActive;

	Button []menuButtons;
	int activeMenuButtonIndex;
	
	bool statusMenu = false;

	bool oneControllerDirectionUse = false;

	public bool StatusMenu {
		get{ return statusMenu; }
	}

	// Use this for initialization
	void Start () {

		//informativeMan = UtilFinder._GetComponentOfGameObjectWithTag<InformativeManager> ("Controller");

		initializeReferencesOfMenu ();

	}

	void initializeReferencesOfMenu() {

		GeneralFinder.informativeManager.c_initializeInformative ();
		//informativeMan.c_initializeInformative ();

		initializeIntroOfMenu ();

	}

	void initializeIntroOfMenu() {

		if (GeneralFinder.canvasMenu != null) {

			foreach(Transform child in GeneralFinder.canvasMenu.transform) {

				if(child.name=="MainPanel-Intro") {

					canvasIntro = child.gameObject;
				}

				if(child.name=="MainPanel-Informative") {

					canvasInformative = child.gameObject;
				}

			}

			if(canvasIntro== null){
				
				Debug.Log ("ATTENZIONE - MainPanel-Intro non trovato dentro CanvasMenu!");
				return;
			}

			bool statusCanvasIntro = canvasIntro.activeSelf;


			GeneralFinder.canvasMenu.SetActive(true);
			canvasIntro.SetActive(true);
			
			foreach (Transform child in canvasIntro.transform) {

				if(child.name=="Center") {

					menuButtons = child.GetComponentsInChildren<Button>();

					//initialize menu buttons func
					initializeMenuButtons();



				}

			}

			canvasIntro.SetActive(statusCanvasIntro);
			GeneralFinder.canvasMenu.SetActive(false);

		}
		else {

			Debug.Log ("ATTENZIONE - canvasMenu assegnato a MenuManager è null");

		}

	}

	void initializeMenuButtons() {

		int indexButt = 0;

		foreach(Button butt in menuButtons) {
			//Debug.Log ("ecco cosa ho trovato " + butt.gameObject.name);

			int tempInd = indexButt;

			if(butt.gameObject.name=="Return") {

				butt.onClick.AddListener(() => { returnToPlay(false); });

				indexButt++;
			}

			if(butt.gameObject.name=="RestartLevel") {
				
				butt.onClick.AddListener(() => { reloadThisLevel(); });
				indexButt++;
			}
			
			if(butt.gameObject.name=="Items") {

				butt.onClick.AddListener(() => { openInformativeSection(); });
				indexButt++;
			}
			
			if(butt.gameObject.name=="Options") {

				butt.onClick.AddListener(() => { openOptionsSection(); });
				indexButt++;
			}

			if(butt.gameObject.name=="Save Progress") {
				
				butt.onClick.AddListener(() => { GeneralFinder.informativeManager.c_saveData(); });
				indexButt++;
			}
			
			if(butt.gameObject.name=="Exit") {

				butt.onClick.AddListener(() => { exitFromLevel(); });
				indexButt++;
			}


			
		}

	}



	#region CALLBACKS

	public void returnToPlay(bool exitMenu) {

		c_enableMenu(exitMenu);

		GeneralFinder.playingUILateral.showIconsImmediately(PlayingUILateral.UIPosition.Right , exitMenu);
		GeneralFinder.playingUILateral.showIconsImmediately(PlayingUILateral.UIPosition.Left , exitMenu);

	}

	public void openInformativeSection() {

		c_switchMenuSection (canvasIntro, canvasInformative);

	}

	public void openOptionsSection() {
		
	}

	public void exitFromLevel() {

		Application.LoadLevel ("00_startMenu");

	}

	public void reloadThisLevel() {

		Application.LoadLevel (Application.loadedLevel);
		PlayStatusTracker.inPlay = true;
		//GeneralFinder.playStatusTracker.inPlay = true;
	}

	public void c_enableMenu(bool enable, GameObject canvasToShow=null) {
		
		if (enable) {
			//blocco input
			
			//Time.timeScale = 0.0f;
			PlayStatusTracker.inPlay = false;
			//GeneralFinder.playStatusTracker.inPlay = false;
			//Debug.Log("setto in playmode");

			if(canvasToShow==null)
				c_switchMenuSection(null, canvasIntro);
			else
				c_switchMenuSection(null, canvasToShow);
		} 
		else {
			//sblocco input
			
			//Time.timeScale = 1.0f;
			PlayStatusTracker.inPlay = true;
			//GeneralFinder.playStatusTracker.inPlayMode = true;
			c_switchMenuSection(canvasIntro, null);
			
		}
		
	}

	public bool getStatusMenu() {

		return statusMenu;

	}

	public GameObject getCanvasActive() {

		return canvasActive;

	}

	public GameObject getCanvasInformative() {
		
		return canvasInformative;
		
	}

	public bool isInformativeCanvasActive() {

		if (canvasActive == canvasInformative)
			return true;
		else
			return false;

	}

	#endregion CALLBACKS

	// Update is called once per frame
	void Update () {


		//catturo l'evento se
		//1) il menu è tutto spento
		//2) opp se è attivo sia il menu che intro
		//3) non lo catturo se è attivo il menu e qualcosa diverso da intro


		if(GeneralFinder.inputManager.getButtonDown("Start") && !GeneralFinder.informativeManager.invokeWithoutMenu ) {
			//Debug.Log("ahi");
			if(!GeneralFinder.canvasMenu.activeSelf || canvasActive == canvasIntro) {
				//caso in cui siamo in game o dentro la scheda intro del menu
				//quindi o stiamo entrando in pausa adesso, o ne stiamo uscendo

				//Debug.Log("apro/chiudo menu");

				returnToPlay(!statusMenu);

			}
			else {
				//caso di navigazione dentro al menu, quindi di ritorno alla scheda intro
				//Debug.Log("switcho sezione menu");
				c_switchMenuSection(canvasActive, canvasIntro);

			}

		}



		if (!statusMenu)
			return;

		//Debug.Log("menu");

		if (GeneralFinder.inputManager.getAxisRaw("Vertical") != 0.0f) {
			//Debug.Log("menu");
			if(!oneControllerDirectionUse) {

				oneControllerDirectionUse = true;

				float fl = GeneralFinder.inputManager.getAxisRaw("Vertical");

				menuButtons[activeMenuButtonIndex].gameObject.GetComponent<Image>().color = menuButtons[activeMenuButtonIndex].colors.normalColor;

				if(fl > 0.0f) {
					//Debug.Log("menu up");
					activeMenuButtonIndex--;

					if(activeMenuButtonIndex<0) {
						activeMenuButtonIndex= menuButtons.Length-1;
					}
				}
				else {
					//Debug.Log("menu down");
					activeMenuButtonIndex++;

					if(activeMenuButtonIndex>menuButtons.Length-1) {
						activeMenuButtonIndex=0;
					}
				}

				menuButtons[activeMenuButtonIndex].gameObject.GetComponent<Image>().color = menuButtons[activeMenuButtonIndex].colors.highlightedColor;
			}
		}
		else {

			oneControllerDirectionUse = false;

		}


		if(GeneralFinder.inputManager.getButtonUp("Interaction") || GeneralFinder.inputManager.getButtonUp("Jump")) {

			PointerEventData pointer = new PointerEventData(EventSystem.current);

			ExecuteEvents.Execute<IPointerClickHandler>(menuButtons[activeMenuButtonIndex].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);

		}

	}

	public void c_overMouseOnMenuButton(string name) {
		
		foreach (Button butt in menuButtons) {
			
			if(butt.gameObject.name!=name) {
				
				butt.gameObject.GetComponent<Image>().color = butt.colors.normalColor;
				
			}
			
		}
		
	}

	public void c_deselectOtherButtons(int selectIndex) {

		for(int i=0; i< menuButtons.Length; i++) {

			if(i!=selectIndex)
				menuButtons[i].gameObject.GetComponent<Image>().color = menuButtons[i].colors.highlightedColor;

		}

	}
	

	public void c_switchMenuSection(GameObject toDeactivate, GameObject toActivate ) {

		if (!GeneralFinder.canvasMenu.activeSelf) {
			GeneralFinder.canvasMenu.SetActive (true);

		}

		if (toDeactivate != null) {

			toDeactivate.SetActive (false);

		}
		else {
			//se non devo disattivare nulla, vuol dire che il menu è aperto per la prima volta
			statusMenu = true;

		}

		if (toActivate != null) {
			toActivate.SetActive (true);
			canvasActive = toActivate;

			if(canvasActive==canvasInformative) {

				GeneralFinder.unlockableContentUI.stopPulsing();

				GeneralFinder.informativeManager.fillNavigation ();

				GeneralFinder.informativeManager.fillMultimediaAndDetails();
				//GeneralFinder.informativeManager.fillMultimedia();
				//GeneralFinder.informativeManager.fillDetail();

			}

		} 
		else {
			//se non devo attivare nulla, sto uscendo dal menu
			GeneralFinder.canvasMenu.SetActive (false);
			canvasActive = null;
			statusMenu = false;
		}

	}

}
