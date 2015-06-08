using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	PlayStatusTracker statusTracker;
	InformativeManager informativeMan;

	public GameObject canvasMenu;

	public GameObject canvasIntro;
	public GameObject canvasInformative;

	public GameObject canvasActive;

	Button []menuButtons;

	bool statusMenu = false;

	// Use this for initialization
	void Start () {

		informativeMan = UtilFinder._GetComponentOfGameObjectWithTag<InformativeManager> ("Controller");

		statusTracker = UtilFinder._GetComponentOfGameObjectWithTag<PlayStatusTracker> ("Controller");

		initializeReferencesOfMenu ();

	}

	void initializeReferencesOfMenu() {

		informativeMan.c_initializeInformative ();

		initializeIntroOfMenu ();

	}

	void initializeIntroOfMenu() {

		if (canvasMenu != null) {

			foreach(Transform child in canvasMenu.transform) {

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


			canvasMenu.SetActive(true);
			canvasIntro.SetActive(true);
			
			foreach (Transform child in canvasIntro.transform) {

				if(child.name=="Center") {

					menuButtons = child.GetComponentsInChildren<Button>();

					//initialize menu buttons func
					initializeMenuButtons();



				}

			}

			canvasIntro.SetActive(statusCanvasIntro);
			canvasMenu.SetActive(false);

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
		
	}

	public void reloadThisLevel() {

		Application.LoadLevel (Application.loadedLevel);
		statusTracker.inPlayMode = true;
	}

	public void c_enableMenu(bool enable) {
		
		if (enable) {
			//blocco input
			
			//Time.timeScale = 0.0f;
			statusTracker.inPlayMode = false;
			c_switchMenuSection(null, canvasIntro);
			
		} 
		else {
			//sblocco input
			
			//Time.timeScale = 1.0f;
			statusTracker.inPlayMode = true;
			c_switchMenuSection(canvasIntro, null);
			
		}
		
	}

	#endregion CALLBACKS

	// Update is called once per frame
	void Update () {

		//catturo l'evento se
		//1) il menu è tutto spento
		//2) opp se è attivo sia il menu che intro
		//3) non lo catturo se è attivo il menu e qualcosa diverso da intro

		if ( Input.GetKeyUp (KeyCode.Escape)  ) {


			if(!canvasMenu.activeSelf || canvasActive == canvasIntro) {
				//caso in cui siamo in game o dentro la scheda intro del menu
				//quindi o stiamo entrando in pausa adesso, o ne stiamo uscendo
				c_enableMenu(!statusMenu);
			}
			else {
				//caso di navigazione dentro al menu, quindi di ritorno alla scheda intro
				c_switchMenuSection(canvasActive, canvasIntro);

			}

		}

	}


	

	public void c_switchMenuSection(GameObject toDeactivate, GameObject toActivate ) {

		if (!canvasMenu.activeSelf) {
			canvasMenu.SetActive (true);

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
		} 
		else {
			//se non devo attivare nulla, sto uscendo dal menu
			canvasMenu.SetActive (false);
			canvasActive = null;
			statusMenu = false;
		}

	}

}
