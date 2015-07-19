using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	InformativeManager informativeMan;

	//public GameObject canvasMenu;

	GameObject canvasIntro;
	GameObject canvasInformative;

	GameObject canvasActive;

	Button []menuButtons;
	
	bool statusMenu = false;

	// Use this for initialization
	void Start () {

		informativeMan = UtilFinder._GetComponentOfGameObjectWithTag<InformativeManager> ("Controller");

		initializeReferencesOfMenu ();

	}

	void initializeReferencesOfMenu() {

		informativeMan.c_initializeInformative ();

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

		foreach(Button butt in menuButtons) {
			//Debug.Log ("ecco cosa ho trovato " + butt.gameObject.name);
			if(butt.gameObject.name=="Return") {

				butt.onClick.AddListener(() => { returnToPlay(); });
				
			}

			if(butt.gameObject.name=="RestartLevel") {
				
				butt.onClick.AddListener(() => { reloadThisLevel(); });
				
			}
			
			if(butt.gameObject.name=="Items") {

				butt.onClick.AddListener(() => { openInformativeSection(); });

			}
			
			if(butt.gameObject.name=="Options") {

				butt.onClick.AddListener(() => { openOptionsSection(); });

			}

			if(butt.gameObject.name=="Save Progress") {
				
				butt.onClick.AddListener(() => { GeneralFinder.informativeManager.c_saveData(); });
				
			}
			
			if(butt.gameObject.name=="Exit") {

				butt.onClick.AddListener(() => { exitFromLevel(); });

			}
			
		}

	}

	#region CALLBACKS

	public void returnToPlay() {

		c_enableMenu(false);


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

		if ( Input.GetKeyUp (KeyCode.Escape)  ) {


			if(!GeneralFinder.canvasMenu.activeSelf || canvasActive == canvasIntro) {
				//caso in cui siamo in game o dentro la scheda intro del menu
				//quindi o stiamo entrando in pausa adesso, o ne stiamo uscendo
				c_enableMenu(!statusMenu);

				GeneralFinder.playingUILateral.showIconsImmediately(PlayingUILateral.UIPosition.Right , statusMenu);
				GeneralFinder.playingUILateral.showIconsImmediately(PlayingUILateral.UIPosition.Left , statusMenu);

			}
			else {
				//caso di navigazione dentro al menu, quindi di ritorno alla scheda intro
				c_switchMenuSection(canvasActive, canvasIntro);

			}

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

				GeneralFinder.informativeManager.fillNavigation ();
				GeneralFinder.informativeManager.fillMultimedia();
				GeneralFinder.informativeManager.fillDetail();

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
