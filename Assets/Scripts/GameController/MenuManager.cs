﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Gestisce il menù di pausa, quindi l'interruzione di gioco. Comunica con altri script, come InformativeManager e PlayStatusTracker.
/// </summary>

// Dario

public class MenuManager : MonoBehaviour {

	public Texture2D mouseCursor;

	GameObject canvasIntro;
	GameObject canvasInformative;

	GameObject canvasActive;

	Button []menuButtons;
	int activeMenuButtonIndex;
	
	bool statusMenu = false;

	bool oneControllerDirectionUse = false;

	bool oneControllerDirectionAnUse = false;
	bool oneControllerDirectionDigUse = false;

	public bool StatusMenu {
		get{ return statusMenu; }
	}

	// Use this for initialization
	void Start () {

		initializeReferencesOfMenu ();

	}

	void initializeReferencesOfMenu() {

		GeneralFinder.informativeManager.c_initializeInformative ();

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

	public void returnToPlay(bool openMenu) {

		c_enableMenu(openMenu);

		GeneralFinder.playingUILateral.showIconsImmediately(PlayingUILateral.UIPosition.Right , openMenu);
		GeneralFinder.playingUILateral.showIconsImmediately(PlayingUILateral.UIPosition.Left , openMenu);

		handleMouseOnPause ();

	}

	public void openInformativeSection() {

		c_switchMenuSection (canvasIntro, canvasInformative);

	}

	public void openOptionsSection() {
		
	}

	public void exitFromLevel() {

		Application.LoadLevel (0);

	}

	public void reloadThisLevel() {

		Application.LoadLevel (Application.loadedLevel);
		PlayStatusTracker.inPlay = true;

	}

	public void c_enableMenu(bool enable, GameObject canvasToShow=null) {
		
		if (enable) {
			//blocco input
			
			PlayStatusTracker.inPlay = false;

			if(canvasToShow==null)
				c_switchMenuSection(null, canvasIntro);
			else
				c_switchMenuSection(null, canvasToShow);
		} 
		else {
			//sblocco input
			
			PlayStatusTracker.inPlay = true;

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

	void checkOpenAndClose() {

		//catturo l'evento se
		//1) il menu è tutto spento
		//2) opp se è attivo sia il menu che intro
		//3) non lo catturo se è attivo il menu e qualcosa diverso da intro

		if(GeneralFinder.inputManager.getButtonDown("Start") && !GeneralFinder.informativeManager.invokeWithoutMenu 
		   ||  (GeneralFinder.inputManager.getButtonUp("GoBackMenu") && statusMenu ) ) {
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

	}



	void checkMenuNav() {
		
		if(GeneralFinder.inputManager.getButtonDown("UpMenu")) {

			foreach(Button butt in menuButtons) {
				
				butt.gameObject.GetComponent<Animator>().SetTrigger("Normal");
				
			}

			activeMenuButtonIndex--;
			
			if(activeMenuButtonIndex<0) {
				activeMenuButtonIndex= menuButtons.Length-1;
			}

			menuButtons[activeMenuButtonIndex].gameObject.GetComponent<Animator>().SetTrigger("Highlighted");


		}
		else if(GeneralFinder.inputManager.getButtonDown("DownMenu")) {

			foreach(Button butt in menuButtons) {
				
				butt.gameObject.GetComponent<Animator>().SetTrigger("Normal");
				
			}

			activeMenuButtonIndex++;
			
			if(activeMenuButtonIndex>menuButtons.Length-1) {
				activeMenuButtonIndex=0;
			}

			menuButtons[activeMenuButtonIndex].gameObject.GetComponent<Animator>().SetTrigger("Highlighted");

		}
	}
	

	void checkInteraction() {

		if(GeneralFinder.inputManager.getButtonUp("Interaction") || GeneralFinder.inputManager.getButtonUp("GoMenu")) {
			
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			
			ExecuteEvents.Execute<IPointerClickHandler>(menuButtons[activeMenuButtonIndex].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
			
		}

	}


	void handleMouseOnPause() {

		CursorMode cursorMode = CursorMode.Auto;
		Vector2 hotSpot = Vector2.zero;

		if (PlayStatusTracker.inPlay)
			Cursor.visible = false;
		else {
			Cursor.SetCursor(mouseCursor, hotSpot, cursorMode);
			Cursor.visible = true;

		}

	}
	// Update is called once per frame
	void Update () {

		//handleMouseOnPause ();

		checkOpenAndClose();

		if (!statusMenu)
			return;

		checkMenuNav ();

		checkInteraction ();

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

				//GeneralFinder.unlockableContentUI.stopPulsing();

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
