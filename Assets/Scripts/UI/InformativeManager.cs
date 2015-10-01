using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using System;

public enum infoContentType {
	Fragments,
	Collectibles,
	FunFacts,
}

public enum DataManagement {
	Load, //prova a caricare, se non c'è l'xml va a useactual, 
	UseActual,//non fa nulla e si attiene alle impostazioni da inspector
	Save,//come useactual, in più salva il file
}

public class InformativeManager : MonoBehaviour {

	#region VARIABLES

	#region PUBLICVARIABLES

	//public GameObject canvasMenu;
	GameObject canvasInformative;
	GameObject canvasIntro;

	public bool loadDefaultConf;

	public DataManagement dataManage;

	[SerializeField]
	public InformativeSection []sections;

	int activeSection;

	public int ActiveSection {
		get{ return activeSection;}

	}

	[HideInInspector]
	public bool invokeWithoutMenu = false;

	public int actualLevelNumber = -1;

	[HideInInspector]
	public float timeCanShowNewContent = 5.0f;

	bool verticalDirectionDigUse = false;

	bool verticalDirectionAnUse = false;

	bool horizontalDirectionDigUse = false;

	bool horizontalDirectionAnUse = false;

	bool cursorVerticalDirectionUse = false;
	
	bool cursorHorizontalDirectionUse = false;

	public bool controllerConfig1;

	//[HideInInspector]
	public bool canOpenInformative = true;


	#endregion PUBLICVARIABLES

	#region PRIVATEVARIABLES
	
	//MenuManager menuMan;

	GameObject multimediaSection;
	Image multimedia;
	Button[] multimediaButtons = new Button[2];
	
	GameObject navigationSection;
	const int maxItems = 4;
	Text sectionName;
	Button[] sectionButtons = new Button[2];
	GameObject []iconItems = new GameObject[maxItems];
	Image []iconImages = new Image[maxItems];
	Button[]iconButtons = new Button[maxItems];

	GameObject helpSection;

	Image changeContentHelp;

	Image changeSubContentHelp;

	Image []changeSectionHelp;
	Image changeSectionPCHelp;

	Image exitInformativeHelp;


	GameObject detailSection;
	Text detail;
	Text indexSubContent;

	Button exitButton;

	bool unlockedNewContent = false;

	bool initialized = false;
	

	#endregion PRIVATEVARIABLES

	#endregion VARIABLES

	void Awake() {

		switch (dataManage) {

		case DataManagement.Load:
			InfoSectionContainer.tryLoadInformativeManagerConf (ref sections, InfoSectionContainer.defaultFileName + "-0-");
			break;

		case DataManagement.UseActual:
			break;
		case DataManagement.Save:
			c_saveData();
			break;

		}

		/*
		if (loadDefaultConf) {
			
			InfoSectionContainer.loadInformativeManagerConf(ref sections);
			
		}
		else {
			
			InfoSectionContainer.tryLoadInformativeManagerConf (ref sections, InfoSectionContainer.defaultFileName + "-0-");
			
		}
		*/

		setLevelNumberAndUnlockerOfThisLevel ();

	}


	void Start () {

		initializeUIReferences ();

	}

	#region PRIVATEMETHODS

	#region STARTMETHODS

	void setLevelNumberAndUnlockerOfThisLevel() {
		
		//scorrere le sezioni
		
		bool levelFound = false;

		foreach (InformativeSection section in sections) {
			
			foreach(InformativeContent cont in section.contents) {
				
				if(cont.unlockerObject !=null) {
					levelFound = true;
					actualLevelNumber = section.levelN;
					break;
					
				}
				
			}

			if(levelFound)
				break;

		}

		if (actualLevelNumber < 0 || !levelFound) {
			Debug.Log ("ATTENZIONE - NESSUN unlocker assegnato nella scena");
			return;
		}

		int sectionIndex = 0;

		foreach (InformativeSection section in sections) {

			int contentIndex = 0;

			if(section.levelN == actualLevelNumber){

				switch(section.contentType) {
				case infoContentType.Collectibles :
					foreach (InformativeContent conte in section.contents) {

						if(conte.unlockerObject!=null) {
							if(!conte.lockedContent) {
								
								conte.unlockerObject.GetComponent<UnlockContent>().c_contentUnlocked();

							}
							else {

								conte.unlockerObject.GetComponent<UnlockContent>().c_setContentInt(contentIndex);
								conte.unlockerObject.GetComponent<UnlockContent>().c_setSectionInt(sectionIndex);
							
							}
							
							
						}

						//TODO:

						if(conte.questionObject!=null) {

							if(!conte.questionAnswered) {

								conte.questionObject.GetComponent<NPCSurveyInfo>().c_setContentInt(contentIndex);
								conte.questionObject.GetComponent<NPCSurveyInfo>().c_setSectionInt(sectionIndex);

							}

						}

						contentIndex++;
					}

					break;

				case infoContentType.FunFacts :
					foreach (InformativeContent funfact in section.contents) {
						
						if(funfact.unlockerObject!=null) {
							if(!funfact.lockedContent) {
								
								funfact.unlockerObject.GetComponent<UnlockContent>().c_contentUnlocked();
								
							}
							else {

								funfact.unlockerObject.GetComponent<UnlockContent>().c_setContentInt(contentIndex);
								funfact.unlockerObject.GetComponent<UnlockContent>().c_setSectionInt(sectionIndex);
							
							}
							
							
						}
							
						
						contentIndex++;
					}
					break;
					
				case infoContentType.Fragments :
					foreach (InformativeContent fragme in section.contents) {
						
						if(fragme.unlockerObject!=null) {
							if(!fragme.lockedContent) {
								
								fragme.unlockerObject.SetActive(false);
								
							}
							else {
								/*
								if(fragme.unlockerObject.activeSelf) {
									fragme.unlockerObject.SendMessage("c_setSectionInt", sectionIndex);
									fragme.unlockerObject.SendMessage("c_setFragmentInt", contentIndex);
								}
								*/
							}
							
							
						}
						
						contentIndex++;
					}
					break;
					

				}

			}

			sectionIndex++;

		}

		//trovare quella con degli unlocker != null
		//disattivarli o meno in base al fatto che siano già stati scoperti
		
	}



	#region GETGRAPHICREF

	void initializeUIReferences() {

		if (initialized)
			return;

		//menuMan = UtilFinder._GetComponentOfGameObjectWithTag<MenuManager> ("Controller");


		if (GeneralFinder.canvasMenu != null) {

			//bool activeState = canvasMenu.activeSelf;



			if (getCanvas ()) {


				GeneralFinder.canvasMenu.SetActive (true);
				activeTempCanvasInformative(true);

				if(getSections()) {

					getNavigationComponents ();
					
					getMultimediaComponents ();
					
					getDetailComponents ();

					getHelpComponents();

					fillMultimediaAndDetails(activeSection, 0);

					//fillDetail (activeSection, 0);
					//fillMultimedia (activeSection, 0);
					
					fillNavigation (activeSection);

				}

				activeTempCanvasInformative (false);
				
				GeneralFinder.canvasMenu.SetActive (false);


			} else {

				//TODO:
				
			}



			
		} else {

			Debug.Log ("ATTENZIONE - canvasMenu non assegnato a InformativeManager");

		}

		initialized = true;

	}

	void getNavigationComponents() {


		foreach (Transform child in navigationSection.transform) {
			//Debug.Log ("sto esplorando " + child.name);
			if(child.name=="Title") {

				sectionName = child.gameObject.GetComponentInChildren<Text>();

			}

			if(child.name=="Switcher"){

				foreach(Transform nephew in child) {
					if(nephew.name=="Left") {
						sectionButtons[0] = nephew.GetComponent<Button>();
						sectionButtons[0].onClick.AddListener(() => { c_changeSection(false); });

					}

					if(nephew.name=="Right") {
						sectionButtons[1] = nephew.GetComponent<Button>();
						sectionButtons[1].onClick.AddListener(() => { c_changeSection(true); });

					}
				}
			}

			if(child.name=="Contents") {

				int i=0;
				foreach(Transform nephew in child) {

					//TODO: provvisorio
					if(i>=maxItems)
						break;

					iconItems[i] = nephew.gameObject;
					iconImages[i] = iconItems[i].GetComponentInChildren<Image>();
					iconButtons[i] = iconItems[i].GetComponentInChildren<Button>();
					//Debug.Log("ho preso " + iconButtons[i].ToString());
					int temp = i;
					//
					if(iconButtons[i] != null)
						iconButtons[i].onClick.AddListener(() => { c_changeContent(temp); });
					else {
						Debug.Log ("siamo alla " + i);
						break;
					}
					i++;

				}

			}


		}


	}

	void getMultimediaComponents() {

		foreach (Transform child in multimediaSection.transform) {

			if(child.name=="BodyMedia") {

				foreach (Transform nephew in child.transform) {

					if(nephew.name=="Media") {

						multimedia = nephew.gameObject.GetComponentInChildren<Image> ();
					}

					if(nephew.name=="Switcher") {

						foreach(Transform newbie in nephew) {

							if(newbie.name=="Left") {

								multimediaButtons[0] = newbie.GetComponent<Button>();
								multimediaButtons[0].onClick.AddListener(() => { c_changeSubContent(false); });

							}
							
							if(newbie.name=="Right") {

								multimediaButtons[1] = newbie.GetComponent<Button>();
								multimediaButtons[1].onClick.AddListener(() => { c_changeSubContent(true); });

							}

						}

					}

					if(nephew.name=="Index") {
						
						//multimedia = nephew.gameObject.GetComponentInChildren<Image> ();
						indexSubContent = nephew.gameObject.GetComponentInChildren<Text>();
					}
				}
				//multimediaButtons

			}

			if(child.name=="LateralMedia") {

				foreach (Transform nephew in child.transform) {
					
					if(nephew.name=="High") {

						exitButton = nephew.GetComponentInChildren<Button>();
						exitButton.onClick.AddListener(() => { handleIntroAndExitInformativeMenu(); });
					}

				}
			}
		}

		if (multimedia == null) {

			Debug.Log ("ATTENZIONE - immagine dentro multimedia section è null");
		}

	}

	void getDetailComponents() {

		foreach (Transform child in detailSection.transform) {
			
			if(child.name=="BodyDetail") {
				detail = child.gameObject.GetComponentInChildren<Text> ();

				break;
			}
		}

		if (detail == null) {
			
			Debug.Log ("ATTENZIONE - testo dentro detail section è null");
		}

	}

	bool getCanvas() {

		foreach (Transform child in GeneralFinder.canvasMenu.transform) {
			if(child.name=="MainPanel-Informative") {
				canvasInformative = child.gameObject;
			}
			
			if(child.name=="MainPanel-Intro") {
				canvasIntro = child.gameObject;
			}
		}
		
		if(canvasInformative== null){
			
			Debug.Log ("ATTENZIONE - MainPanel-Informative non trovato dentro CanvasMenu!");
			return false;
		}

		if(canvasIntro== null){
			
			Debug.Log ("ATTENZIONE - MainPanel-Intro non trovato dentro CanvasMenu!");
			return false;
		}

		return true;

	}

	bool getSections(){

		foreach (Transform child in canvasInformative.transform) {

			if(child.name=="Multimedia") {

				multimediaSection = child.gameObject;

			}

			if(child.name=="Detail") {
				
				detailSection = child.gameObject;
				
			}

			if(child.name=="Navigation") {
				
				navigationSection = child.gameObject;
				
			}

			if(child.name=="Help") {
				
				helpSection = child.gameObject;
				
			}

		}

		if(navigationSection== null){
			
			Debug.Log ("ATTENZIONE - navigationSection non trovato dentro MainPanel!");
			return false;
		}

		if(detailSection== null){
			
			Debug.Log ("ATTENZIONE - detailSection non trovato dentro MainPanel!");
			return false;
		}

		if(multimediaSection== null){
			
			Debug.Log ("ATTENZIONE - multimediaSection non trovato dentro MainPanel!");
			return false;
		}

		if(helpSection== null){
			
			Debug.Log ("ATTENZIONE - helpSection non trovato dentro MainPanel!");
			return false;
		}

		return true;

	}

	void getHelpComponents() {

		foreach (Transform child in helpSection.transform) {

			/*
			if(child.name.Contains("changeContentH")) {
				//Debug.Log("ok cont");
				changeContentHelp = child.gameObject.GetComponentInChildren<Image>();

			}

			if(child.name.Contains("changeSubContentH")) {
				//Debug.Log("ok sub");
				changeSubContentHelp = child.gameObject.GetComponentInChildren<Image>();

			}
			*/

			if(child.name.Contains("changeSectionH")) {
				//Debug.Log("ok sec");
				Image []tempImgs = child.GetComponentsInChildren<Image>();

				changeSectionHelp = new Image[2];

				foreach(Image im in tempImgs) {

					if(im.gameObject.transform.parent.gameObject.name.Contains("left")) {

						changeSectionHelp[0] = im;

					}

					if(im.gameObject.transform.parent.gameObject.name.Contains("right")) {
						
						changeSectionHelp[1] = im;
						
					}

					if(im.gameObject.transform.parent.gameObject.name.Contains("pc")) {

						changeSectionPCHelp = im;

					}

				}

			}

			if(child.name.Contains("exit")) {
				//Debug.Log("ok sub");
				exitInformativeHelp = child.gameObject.GetComponentInChildren<Image>();
				
			}

		}

	}

	#endregion GETGRAPHICREF

	#endregion STARTMETHODS






	void activeTempCanvasInformative(bool act) {

		canvasInformative.SetActive (act);

	}

	void handleIntroAndExitInformativeMenu() {

		//Debug.Log ("INVOCATO");

		if( (!canvasInformative.activeSelf || !canvasInformative.transform.parent.gameObject.activeSelf  ) 
	     	/*&& GeneralFinder.inputManager.getButtonDown("OpenInformative") */
	   		&& !invokeWithoutMenu 
	   		&& !GeneralFinder.menuManager.StatusMenu) {

			//Debug.Log("ciao0");
			
			//GeneralFinder.unlockableContentUI.stopPulsing();
			
			invokeWithoutMenu = true;
			PlayStatusTracker.inPlay = false;

			fillNavigation ();
			
			fillMultimediaAndDetails();

			canvasInformative.transform.parent.gameObject.SetActive(true);
			canvasInformative.SetActive(true);

			if(unlockedNewContent) {
				
				GeneralFinder.testInformativeManager.c_setShowWhenUnlocked(activeSection, sections[activeSection].activeContent);
				unlockedNewContent = false;
				
			}
			
			
		}
		else {
			//Debug.Log("ciao1");
			if(!GeneralFinder.menuManager.StatusMenu && invokeWithoutMenu) {
				//Debug.Log("ciao2");
				
				PlayStatusTracker.inPlay = true;
				
				canvasInformative.SetActive(false);
				canvasInformative.transform.parent.gameObject.SetActive(false);
				
				StartCoroutine(countDownMenu());
			}
			else {
				//caso SOLO per X di informative schedules, premuto per tornare al menu
				if(GeneralFinder.menuManager.StatusMenu && !invokeWithoutMenu) {
					//Debug.Log("ciao3");

					GeneralFinder.menuManager.c_switchMenuSection (canvasInformative, canvasIntro);

				}

			}
			
		}

	}

	void checkOpenAndClose() {

		if((GeneralFinder.inputManager.getButtonUp("Start") && invokeWithoutMenu) 
		   || (GeneralFinder.inputManager.getButtonUp("GoBackMenu") && canvasInformative.activeSelf && invokeWithoutMenu)
		   || GeneralFinder.inputManager.getButtonDown("OpenInformative")) {
			
			//Debug.Log("premuto qualcosa per inf");

			handleIntroAndExitInformativeMenu();

			handleMouseOnPause();

		}

	}

	void Update() {

		if (!canOpenInformative)
			return;

		checkOpenAndClose ();

		if (!canvasInformative.activeSelf) 
			return;


		controllerNavigation ();

		
	}

	#region CONTROLLERNAV

	void checkSectionNav() {

		if (GeneralFinder.inputManager.getButtonDown ("RightMenu")) {

			controllerChangeSection(1.0f);

		}
		else if (GeneralFinder.inputManager.getButtonDown ("LeftMenu")) {

			controllerChangeSection(-1.0f);

		}

	}

	void checkSectionNav1() {
		
		if (GeneralFinder.inputManager.getButtonDown ("RightMenu")) {
			
			controllerChangeSubContent(1.0f);
			
		}
		else if (GeneralFinder.inputManager.getButtonDown ("LeftMenu")) {
			
			controllerChangeSubContent(-1.0f);
			
		}
		
	}

	void checkContentNav() {

		if(GeneralFinder.inputManager.getButtonDown("UpMenu")) {

			controllerChangeContent(1.0f);

		}
		else if(GeneralFinder.inputManager.getButtonDown("DownMenu")) {

			controllerChangeContent(-1.0f);
		
		}
	}


	void checkSubContentNav() {

		if(GeneralFinder.inputManager.getButtonDown("ForwardTrigger") ) {
			
			//float fl = Input.GetAxisRaw("FrontTrigger");
			
			controllerChangeSubContent(1.0f);
			
		}
		else if (GeneralFinder.inputManager.getButtonDown("BackwardTrigger")) {
			
			controllerChangeSubContent(-1.0f);
			
		}


	}

	void checkSubContentNav1() {
		
		if(GeneralFinder.inputManager.getButtonDown("ForwardTrigger") ) {
			
			//float fl = Input.GetAxisRaw("FrontTrigger");
			
			controllerChangeSection(1.0f);
			
		}
		else if (GeneralFinder.inputManager.getButtonDown("BackwardTrigger")) {
			
			controllerChangeSection(-1.0f);
			
		}
		
		
	}
	

	void setSprites() {

		//GeneralFinder.inputManager.getControllerSprite(

	}

	void controllerNavigation() {

		if (controllerConfig1) {
			checkSectionNav1 ();
			checkSubContentNav1 ();
		} else {
			checkSectionNav ();
			checkSubContentNav ();

		}


		checkContentNav ();


		/*
		if (Input.GetButtonDown ("Horizontal-menu-nav")) {
			
			//float fl = Input.GetAxisRaw("Horizontal-menu-nav");
			
			
		}
		*/

		/*
		if (Input.GetButtonDown ("Vertical-Informative-menu-nav")) {
			
			float fl = Input.GetAxisRaw("Vertical-Informative-menu-nav");
			
			controllerScrollBarInfo(fl);
			
		}
		*/

	}

	void controllerScrollBarInfo(float fl) {

		RectTransform rt = detail.gameObject.GetComponent<RectTransform> ();

		if (fl > 0) {

			rt.offsetMax = new Vector2 (rt.offsetMax.x, rt.offsetMax.y - 30.0f);
			rt.offsetMin = new Vector2 (rt.offsetMax.x, rt.offsetMax.y + 30.0f);


		} else {

			rt.offsetMax = new Vector2 (rt.offsetMax.x, rt.offsetMax.y + 30.0f);
			rt.offsetMin = new Vector2 (rt.offsetMax.x, rt.offsetMax.y - 30.0f);

		}

	}

	void controllerChangeSection(float fl) {

		if(fl>0)
			c_changeSection(true);
		else
			c_changeSection(false);

		c_changeContent (0);

	}

	void controllerChangeSubContent(float fl) {

		if (fl > 0) {
			c_changeSubContent(true);
			//c_changeMultimedia (true);
		}
		else {
			c_changeSubContent(false);
			//c_changeMultimedia (false);
		}
	}

	void controllerChangeContent(float fl) {

		int tempAct = 0;
		int prevAct = 0;

		if (sections [activeSection].lockedSection) {
			Debug.Log("locked sect - return from controllerChangeContent");
			return;
		}

		tempAct = sections [activeSection].activeContent;

		prevAct = tempAct;

		//Debug.Log ("active content>" + tempAct);

		int attempts = 0;

		while(true) {
			if(fl>0)
				tempAct--;
			else
				tempAct++;
			
			if(tempAct >= sections[activeSection].contents.Length) {
				//Debug.Log ("oo" + tempAct);
				tempAct = 0;
			}
			else {
				if(tempAct<0) {
					//Debug.Log ("aa" + tempAct);
					tempAct = sections[activeSection].contents.Length-1;
				}
			}

			//Debug.Log ("active content" + tempAct);

			if(!sections[activeSection].contents[tempAct].lockedContent)
				break;

			attempts++;

			if(attempts>5) {
				//Debug.Log("attempts > 5");
				return;
			}
		}

		c_changeContent(tempAct);


	}

	void handleMouseOnPause() {
		
		CursorMode cursorMode = CursorMode.Auto;
		Vector2 hotSpot = Vector2.zero;
		
		if (PlayStatusTracker.inPlay)
			Cursor.visible = false;
		else {
			Cursor.SetCursor(GeneralFinder.menuManager.mouseCursor, hotSpot, cursorMode);
			Cursor.visible = true;

		}
		
	}

	
	#endregion CONTROLLERNAV

	#endregion PRIVATEMETHODS



	#region CALLBACKS

	public InformativeSection[] getUnlockedSections () {

		int nUnlockSection = 0;

		for(int i=0; i<sections.Length; i++) {

			if(!sections[i].lockedSection)
				nUnlockSection++;

		}

		if (nUnlockSection == 0) {
			Debug.Log("no unlocked section");
			return null;
		}

		InformativeSection []nUnlockedSections = new InformativeSection[nUnlockSection];
		int indexUsed = 0;

		for(int i=0; i<sections.Length; i++) {
			
			if(!sections[i].lockedSection) {

				nUnlockedSections[indexUsed] = sections[i];
				indexUsed++;

			}
			
		}

		return nUnlockedSections;
		
	}

	public SubContent c_getRandomSubContent(){

		InformativeSection [] sections = GeneralFinder.informativeManager.getUnlockedSections ();
		
		if (sections != null) {
			
			int randomSectionIndex = UnityEngine.Random.Range (0, sections.Length);
			
			int randomContentIndex = 0;

			int attempts = 0;

			while (true) {
				
				randomContentIndex = UnityEngine.Random.Range (0, sections [randomSectionIndex].contents.Length);
				
				if (!sections [randomSectionIndex].contents [randomContentIndex].lockedContent) {
					
					break;
					
				}

				attempts++;

				//forma di sicurezza...
				if(attempts>5) {
					Debug.Log ("più di 5 prove - non trovo content sbloccati");
					return null;
				}
				
			}
			int randomSubContentIndex = 0;
			attempts = 0;

			//prendo un subcontent a caso
			while(true) {

				randomSubContentIndex = UnityEngine.Random.Range (0, sections [randomSectionIndex].contents[randomContentIndex].subContents.Length);
				attempts++;

				if(sections [randomSectionIndex].contents[randomContentIndex].subContents.Length-1 >= randomSubContentIndex) {

					break;

				}

				if(attempts>5) {
					Debug.Log ("più di 5 prove - non trovo subcontent adatto");
					return null;
				}

			}

			//verifico che il subcontent sia utilizzabile per il loading, in caso contrario li scorro tutti e prendo il primo compatibile
			if(!sections [randomSectionIndex].contents[randomContentIndex].subContents[randomSubContentIndex].usableForLoading) {
				
				bool solved = false;
				
				for(int i=0; i<sections [randomSectionIndex].contents[randomContentIndex].subContents.Length; i++) {

					if(sections [randomSectionIndex].contents[randomContentIndex].subContents.Length-1 >= i) {
						
						if(sections [randomSectionIndex].contents[randomContentIndex].subContents[i]!= null) {
							
							if(sections [randomSectionIndex].contents[randomContentIndex].subContents[i].usableForLoading) {
								
								randomSubContentIndex = i;
								solved = true;
								break;
								
							}
							
						}
						
					}


					
				}
				
				if(!solved) {
					Debug.Log ("non trovo subContent usabili");
					return null;
					
				}
				
			}
			
			return sections [randomSectionIndex].contents[randomContentIndex].subContents[randomSubContentIndex];
			
			
			
		}
		else {
			Debug.Log ("sections unlocked nulle");
			return null;
		}
		
	}

	
	public void fillNavigation(int sectionN=-5) {
		
		if (sectionN == -5) {
			
			sectionN = activeSection;
			
		}
		
		int index = 0;
		
		int numberOfContents = sections [sectionN].contents.Length;
		
		sectionName.text = sections [sectionN].title;
		
		foreach (GameObject item in iconItems) {
			
			//Debug.Log ("passo l'item " + item.name + " contentsN " + numberOfContents);
			
			if(index >= numberOfContents || sections [sectionN].contents[index]==null)
				break;
			
			item.SetActive(true);
			
			if(!sections [sectionN].contents[index].lockedContent) {
				iconImages[index].sprite = sections [sectionN].contents[index].iconUnlock;
				iconButtons[index].interactable = true;
			}
			else {
				iconImages[index].sprite = sections [sectionN].contents[index].iconLock;
				iconButtons[index].interactable = false;
			}
			
			index++;
			//Debug.Log ("fine passata l'item " + item.name);
		}
		
		for (int rest=index; rest<iconItems.Length; rest++) {
			
			
			iconItems[rest].SetActive(false);
			
		}
		
	}

	public void fillMultimediaAndDetails(int sectionN = -3, int contentN = -1, int subContentN=0) {

		if (sectionN == -3) {
			
			sectionN = activeSection;
			
		}
		
		if (contentN == -1) {
			
			contentN = sections[sectionN].activeContent;
			
		}
		
		//Debug.Log("section " + sectionN + " e content " + contentN);
		
		if (sections [sectionN].contents [contentN] == null){
			Debug.Log ("ATTENZIONE - contenuto nullo");
			return;
		}

		if (!sections [sectionN].contents [contentN].lockedContent && sections [sectionN].contents [contentN].subContents != null) {

			if(sections [sectionN].contents [contentN].subContents.Length-1 >= subContentN) {

				if(sections [sectionN].contents [contentN].subContents[subContentN] != null) {

					sections [sectionN].contents [contentN].activeSubContentIndex = subContentN;

					multimedia.sprite = sections [sectionN].contents [contentN].subContents [subContentN].image;
					multimedia.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

					detail.text = sections [sectionN].contents [contentN].subContents [subContentN].infoText.text;

				}

			}

		}

		//detail.text = sections [sectionN].contents [contentN].infoText.text;
		detail.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0.0f, 0.0f);
		//TODO: adjust lenght of rect based on text lenght
		detail.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0.0f, -400.0f);

		int num = subContentN + 1;

		if(indexSubContent!=null)
			indexSubContent.text = num + "/" + sections [sectionN].contents [contentN].subContents.Length;

	}


	public void c_changeSection(bool forward) {

		int nextIndex = 0;
		int tempActiveSection = activeSection;

		for (int i=0; i<sections.Length; i++) {

			if (forward) {
				tempActiveSection = (tempActiveSection + 1) % sections.Length;
			} else {
				tempActiveSection = (tempActiveSection - 1) % sections.Length;
				if(tempActiveSection<0)
					tempActiveSection = sections.Length-1;
			}

			//Debug.Log ("temp active sect " + tempActiveSection);

			if(!sections[tempActiveSection].lockedSection && sections[tempActiveSection].contentType != infoContentType.Fragments)
				break;

		}

		if (tempActiveSection >= 0) 
			activeSection = tempActiveSection;

		fillNavigation (activeSection);

		fillMultimediaAndDetails (activeSection, sections [activeSection].activeContent);

		if (forward) {
			//Debug.Log("ciaodx");
			sectionButtons[1].gameObject.GetComponent<Animator> ().SetTrigger ("Pressed1");


		} else {
			//Debug.Log("ciaosx");
			sectionButtons[0].gameObject.GetComponent<Animator> ().SetTrigger ("Pressed1");

		}



		//old IMPLEMENTS
		//fillMultimedia (activeSection, sections [activeSection].activeContent);
		//fillDetail (activeSection, sections [activeSection].activeContent);

	}



	public void c_changeSubContent(bool forward) {

		int nextIndex = 0;
		
		int activeContent = sections [activeSection].activeContent;
		int activeSubContent = sections [activeSection].contents [activeContent].activeSubContentIndex;

		int lenSubContents = sections [activeSection].contents [activeContent].subContents.Length;

		if (lenSubContents == 0)
			return;
		//Debug.Log ("len subcontents" + lenImages);
		
		for (int i=0; i< lenSubContents; i++) {
			
			SubContent sc = sections[activeSection].contents[activeContent].subContents[i];
			
			if(sc==null) {

				Debug.Log ("ATTENZIONE - alcune sprite delle immagini multimedia non sono state assegnate, section : " + activeSection + " content : " + activeContent);
				activeSubContent = i;
				break;

			}
			
		}

		if (forward) {
			activeSubContent = (activeSubContent+1)%lenSubContents;

			multimediaButtons[1].gameObject.GetComponent<Animator> ().SetTrigger ("Pressed1");
		} 
		else {

			multimediaButtons[0].gameObject.GetComponent<Animator> ().SetTrigger ("Pressed1");

			activeSubContent = (activeSubContent-1)%lenSubContents;
			
			if(activeSubContent<0)
				activeSubContent = lenSubContents-1;
			
		}

		sections [activeSection].contents [activeContent].activeSubContentIndex = activeSubContent;
		
		fillMultimediaAndDetails (activeSection, sections [activeSection].activeContent, activeSubContent);

	}

	public void c_changeContent(int contentN) {
		//Debug.Log ("cambio a " + contentN);

		if (sections [activeSection].lockedSection)
			return;

		fillMultimediaAndDetails (activeSection, contentN);

		sections [activeSection].activeContent = contentN;

		updateAlphaAndSizeContents (contentN);

	}

	void updateAlphaAndSizeContents(int _activeContent) {

		int index = 0;

		if (unlockedNewContent) {
			//Debug.Log("aaaaaaaaaaaaaaaa" + _activeContent);
			_activeContent = sections[activeSection].activeContent;
			//Debug.Log("aaaaaaaaaaaaaaaa" + _activeContent);
		}
		//Debug.Log("bbbbbbbbbbbbbbbb");

		//Button toactive = null;

		foreach (Button butt in iconButtons) {
			
			if(index!=_activeContent) {
				if(butt != null) {
					//Debug.Log("aaaaaaa>" + index);
					butt.gameObject.GetComponent<Animator> ().SetTrigger ("Normal");
					butt.gameObject.GetComponent<Animator> ().SetBool ("Active", false);

					//butt.image.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
				}
				
			}
			else {
				if(butt != null) {

					Debug.Log("dddddddd>" + index);

					butt.gameObject.GetComponent<Animator> ().SetBool ("Active", true);
					//butt.gameObject.GetComponent<Animator> ().SetTrigger ("Pressed");
					//butt.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
					//toactive = butt;
				}
			}
			
			index++;
			
		}

		//toactive.gameObject.GetComponent<Animator> ().SetBool ("Active", true);

	}

	public void c_activeInformative(bool act) {
		
		if(!GeneralFinder.canvasMenu.activeSelf)
			GeneralFinder.canvasMenu.SetActive(true);
		
		if (act) {
			GeneralFinder.menuManager.c_enableMenu(true);

			GeneralFinder.menuManager.c_switchMenuSection (canvasIntro, canvasInformative);
		} 
		else {
			GeneralFinder.menuManager.c_switchMenuSection (canvasInformative, canvasIntro);
		}
	}

	public void c_initializeInformative(){
		
		initializeUIReferences ();
		
	}

	//UNLOCK CONTENT
	public void c_canShowNewContent(int sect, int cont) {
		
		unlockedNewContent = true;
		
		if (sect >= sections.Length) {
			Debug.Log("ATTENZIONE - section oltre la size durante lo sblocco del contenuto");
			return;
		}

		if (sections [sect].contentType != infoContentType.Collectibles && sections [sect].contentType != infoContentType.FunFacts) {
			Debug.Log("ATTENZIONE - section NON di tipo collectibles o fun facts");
			return;
			
		}
		
		activeSection = sect;
		
		if (cont >= (sections [activeSection].contents.Length)) {
			Debug.Log("ATTENZIONE - contenuto oltre la size durante lo sblocco del contenuto");
			return;
		}

		sections [activeSection].lockedSection = false;
		sections [activeSection].activeContent = cont;


		sections [activeSection].contents[cont].lockedContent = false;

		//TODO: cambiare?
		if (sections [sect].contentType == infoContentType.Collectibles) {

			GeneralFinder.unlockableContentUI.unlockContent (sections [sect].contents [cont].name);
		
		} 
		else {

			GeneralFinder.unlockableContentUI.unlockFact(sections [sect].contents [cont].name);

		}

		StartCoroutine ("countDownShowNewContent");

	}



	public void c_UnlockFragment(int sect, int fragm) {

		if (sect >= sections.Length) {
			Debug.Log("ATTENZIONE - section oltre la size durante lo sblocco del contenuto");
			return;
		}

		if (sections [sect].contentType != infoContentType.Fragments) {
			Debug.Log("ATTENZIONE - section NON di tipo fragments");
			return;

		}
		//activeSection = sect;
		
		if (fragm >= (sections [sect].contents.Length)) {
			Debug.Log("ATTENZIONE - numero del fragment oltre la size durante lo sblocco del contenuto");
			return;
		}



		sections [sect].lockedSection = false;
		
		sections [sect].contents[fragm].lockedContent = false;

		//TODO : cambiare
		GeneralFinder.unlockableContentUI.unlockFragment (sections [sect].contents [fragm].name);

	}

	public void c_addWrongTrial(int sectionN, int contentN) {

		if(sections[sectionN].contents[contentN].questionObject!=null) {

			sections[sectionN].contents[contentN].wrongTrials ++;
		
		}

	}

	public void c_setQuestionAnswered(int sectionN, int contentN) {
		
		if(sections[sectionN].contents[contentN].questionObject!=null) {
			
			sections[sectionN].contents[contentN].questionAnswered = true;
			
		}
		
	}
	

	public void c_saveData() {
		Debug.Log ("save data");
		InfoSectionContainer.saveInformativeManagerConf (sections, true);
		
	}

	public void c_saveData1() {
		Debug.Log ("save data 1");
		InfoSectionContainer.saveInformativeManagerConf (sections, false);

	}


	public InformativeSection getActualCollectiblesSection (int levelN = -1) {

		int levelNumber;

		if (levelN >= 0) {

			levelNumber = levelN;

		} else {

			levelNumber = actualLevelNumber;

		}

		//Debug.Log ("levelN è " + levelNumber);

		foreach (InformativeSection sect in sections) {

			if(sect.contentType== infoContentType.Collectibles && sect.levelN == levelNumber) {

				return sect;

			}

		}

		return null;

	}

	public InformativeSection getActualFragmentSection (int levelN = -1) {

		int levelNumber;
		
		if (levelN >= 0) {
			
			levelNumber = levelN;
			
		} else {
			
			levelNumber = actualLevelNumber;
			
		}

		foreach (InformativeSection sect in sections) {
			
			if(sect.contentType== infoContentType.Fragments && sect.levelN == levelNumber) {
				
				return sect;
				
			}
			
		}
		
		return null;
		
	}

	public InformativeSection getActualFunFactsSection (int levelN = -1) {

		int levelNumber;
		
		if (levelN >= 0) {

			levelNumber = levelN;
			
		} else {
			
			levelNumber = actualLevelNumber;
			
		}

		foreach (InformativeSection sect in sections) {
			
			if(sect.contentType== infoContentType.FunFacts && sect.levelN == levelNumber) {
				
				return sect;
				
			}
			
		}
		
		return null;
		
	}

	public void c_getIndexes(GameObject unlocker, ref int sec, ref int cont, infoContentType contType) {

		int sectionN = 0;
		int contentN = 0;


		foreach (InformativeSection section in sections) {

			if(section.levelN == actualLevelNumber && section.contentType == contType) {

				foreach(InformativeContent content in section.contents) {

					if(content.unlockerObject==unlocker) {

						sec = sectionN;
						cont = contentN;
						return;
					}

					contentN++;
				}

			}

			contentN = 0;
			sectionN++;
		}

	}

	public void c_enableInformativeCanvas() {


	}

	public void c_onEnableInformative() {

		updateAlphaAndSizeContents (0);

	}


	public void c_setHelpImages() {

		//GeneralFinder.inputManager.getSprite

		if (GeneralFinder.cursorHandler.useController) {

			//changeContentHelp.sprite = GeneralFinder.inputManager.getControllerSprite (ButtonController.PS3Button.DPadUp);
			//changeSubContentHelp.sprite = GeneralFinder.inputManager.getControllerSprite (ButtonController.PS3Button.DPadRight);

			if(GeneralFinder.inputManager.getKeyboardSprite(ButtonKeyboardMouse.Button.ESC) == null
			   || exitInformativeHelp == null)
				return;

			exitInformativeHelp.sprite = GeneralFinder.inputManager.getControllerSprite (ButtonController.PS3Button.Cerchio);

			changeSectionHelp [0].enabled = true;
			//changeSectionHelp [0].sprite = GeneralFinder.inputManager.getSprite ("ForwardTrigger");
			changeSectionHelp [1].enabled = true;

			changeSectionHelp [0].sprite = GeneralFinder.inputManager.getControllerSprite (ButtonController.PS3Button.L1);
			//changeSectionHelp [0].sprite = GeneralFinder.inputManager.getSprite ("ForwardTrigger");
			changeSectionHelp [1].sprite = GeneralFinder.inputManager.getControllerSprite (ButtonController.PS3Button.R1);

			changeSectionPCHelp.enabled = false;


		}
		else {

			//changeContentHelp.sprite = GeneralFinder.inputManager.getKeyboardSprite(ButtonKeyboardMouse.Button.ArrowUp);
			//changeSubContentHelp.sprite = GeneralFinder.inputManager.getKeyboardSprite(ButtonKeyboardMouse.Button.ArrowRight);

			if(GeneralFinder.inputManager.getKeyboardSprite(ButtonKeyboardMouse.Button.ESC) == null
			   || exitInformativeHelp == null)
				return;

			exitInformativeHelp.sprite = GeneralFinder.inputManager.getKeyboardSprite(ButtonKeyboardMouse.Button.ESC);

			changeSectionPCHelp.enabled = true;

			changeSectionPCHelp.sprite = GeneralFinder.inputManager.getKeyboardSprite(ButtonKeyboardMouse.Button.TAB);

			changeSectionHelp [0].enabled = false;
			//changeSectionHelp [0].sprite = GeneralFinder.inputManager.getSprite ("ForwardTrigger");
			changeSectionHelp [1].enabled = false;

		}
	}


	#endregion CALLBACKS

	#region COROUTINES

	IEnumerator countDownShowNewContent() {
		
		yield return new WaitForSeconds (timeCanShowNewContent);
		
		//GeneralFinder.playingUI.cleanPositionButtonObject(PlayingUI.UIPosition.UpperRight);
		//GeneralFinder.playingUI.cleanPositionGameObjects(PlayingUI.UIPosition.UpperRight);
		
		unlockedNewContent = false;
		
	}

	IEnumerator countDownMenu() {
		
		yield return new WaitForSeconds (0.2f);
		
		invokeWithoutMenu = false;
		
	}

	#endregion COROUTINES


	
}

[XmlRoot("InfoSectionCollection")]
public class InfoSectionContainer
{

	public static string defaultPath = "Config/InformativeConf/";
	public static string defaultFileName = "InfoFileConf";

	[XmlArray("Sections"),XmlArrayItem("Section")]
	public InformativeSection[] sectionsLoaded;

	public static void saveInformativeManagerConf(InformativeSection []_sections, bool defaultName) {
		
		InfoSectionContainer infoCon = new InfoSectionContainer ();
		infoCon.sectionsLoaded = _sections;

		if (defaultName) {
			infoCon.Save (defaultPath + defaultFileName + "-0-.xml");

		} else {
			string pathToUse = "";

			pathToUse = defaultPath + getAvailableName (defaultPath);

			Debug.Log ("pathtouse >>>" + pathToUse + "<<<");

			infoCon.Save (pathToUse);
		}
		
	}

	static string getAvailableName(string defPath) {

		try {
			string dirName = Path.GetDirectoryName (defPath);
			//string fileName = Path.GetFileName (defPath);
			
			if (!Directory.Exists (dirName)) {
				
				Directory.CreateDirectory(dirName);

				return defaultFileName + "-0-" + ".xml";
			}
			else {

				string []files = Directory.GetFiles(defPath);

				int numAvailable = -1;

				foreach(string fileName in files) {

					if(fileName.Contains(defaultFileName)) {

						string []sep = new string[1];

						sep[0] = "-";

						string []fileNameSplit = fileName.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);

						int num = 0;

						if(Int32.TryParse(fileNameSplit[1], out num)) {

							if(num>numAvailable)
								numAvailable = num+1;

						}


					}

				}

				return defaultFileName + "-" + numAvailable + "-" + ".xml";

			}

		}
		catch(XmlException e) {
			
			throw e;
			
		}
		catch(Exception e) {
			
			throw e;
			
		}
	}

	public void Save(string path)
	{
		//Debug.Log ("ciaone1");
		try {
			string dirName = Path.GetDirectoryName (path);
			string fileName = Path.GetFileName (path);

			Debug.Log ("path" + dirName + " e file " + fileName);

			if (!Directory.Exists (dirName)) {
				//Debug.Log ("ciaone2");
				Directory.CreateDirectory(dirName);
				
			}
			//Debug.Log ("ciaone3");
			var serializer = new XmlSerializer(typeof(InfoSectionContainer));
			using(var stream = new FileStream(path, FileMode.Create))
			{
				//Debug.Log ("ciaone4");
				serializer.Serialize(stream, this);
			}

			DirectoryInfo dir = new DirectoryInfo(dirName);

			FileInfo file = new FileInfo(path);

			dir.Refresh();

			file.Refresh();

		}
		catch(XmlException e) {
			Debug.Log ("ciaoneex1");
			throw e;
			
		}
		catch(Exception e) {
			Debug.Log ("ciaoneex2");
			throw e;
			
		}

	}

	public static void tryLoadInformativeManagerConf(ref InformativeSection []_sections, string fileNameToLoad) {
		

		if (Directory.Exists ("Config/InformativeConf/")) {
			
			try {
				
				string []files = Directory.GetFiles("Config/InformativeConf/");
				
				foreach(string filePath in files) {
					
					string fileName = Path.GetFileName(filePath);
					string fileExt = Path.GetExtension(filePath);
					
					#if _DEBUG
					Debug.Log("file name " + fileName + " fileext " + fileExt);
					#endif

					//TODO: gestire meglio scelta file da caricare
					//magari try catch dentro foreach così che passa al prossimo

					if(fileName.Contains(fileNameToLoad) && fileExt==".xml") {
						
						InfoSectionContainer.loadInformativeManagerConf (ref _sections, filePath);
						Debug.Log("CARICAMENTO RIUSCITO da " + filePath);
						return;
					}
					
				}
				
				
			}
			catch(XmlException e) {

				Debug.Log(e.ToString());

				InfoSectionContainer.loadInformativeManagerConf(ref _sections);
				
				return;
				
			}
			catch(Exception e) {

				Debug.Log(e.ToString());

				InfoSectionContainer.loadInformativeManagerConf(ref _sections);
				
				return;
				
			}

			
		}
		
		
		InfoSectionContainer.loadInformativeManagerConf(ref _sections);

		Debug.Log("NESSUN FILE TROVATO, CARICATE CONFIG DI DEFAULT");
	}
	
	public static void loadInformativeManagerConf(ref InformativeSection []_sections, string _path=null) {
		
		InfoSectionContainer infocon = new InfoSectionContainer ();
		
		if (_path == null) {

			//ORA COME ORA, E' COME SE FOSSE UN USEDEFAULT

			//TextAsset pi = Resources.Load("DefaultInfoFileConf") as TextAsset;
			
			//infocon = InfoSectionContainer.LoadFromText (pi.text);
			
			//infocon.setConfiguration (ref _sections);
			
		}
		else {

			infocon = InfoSectionContainer.Load(_path);
			
			infocon.setConfiguration (ref _sections);
		}
		
		
		
		
		
		//infocon = InfoSectionContainer.Load("/Users/dariorandazzo/Unity Projects/Magic 02/Assets/TextAssets/provainfo.xml");
		
	}

	public void setConfiguration(ref InformativeSection[] sectionsToSet) {


		for (int secToSetInd=0; secToSetInd<sectionsToSet.Length; secToSetInd++) {

			for(int secToLoadInd=0; secToLoadInd<sectionsLoaded.Length; secToLoadInd++) {


				if(sectionsToSet[secToSetInd].title == sectionsLoaded[secToLoadInd].title && 
				   sectionsToSet[secToSetInd].levelN == sectionsLoaded[secToLoadInd].levelN) {

					setConfigurationSection(ref sectionsToSet[secToSetInd], sectionsLoaded[secToLoadInd]);

					break;

				}

			}


		}


	}

	void setConfigurationSection(ref InformativeSection sectionToSet, InformativeSection sectionLoad) {

		//setto le informazioni della section

		sectionToSet.ended = sectionLoad.ended;

		sectionToSet.lockedSection = sectionLoad.lockedSection;

		//setto le informazioni per ogni content

		try {

			for(int contToSetInd =0; contToSetInd< sectionToSet.contents.Length; contToSetInd++) {

				for(int contToLoadInd=0; contToLoadInd< sectionLoad.contents.Length; contToLoadInd++) {

					if(sectionToSet.contents[contToSetInd].name== sectionLoad.contents[contToLoadInd].name) {

						setConfigurationContent(ref sectionToSet.contents[contToSetInd], sectionLoad.contents[contToLoadInd]);
						
						break;

					}
					else {

						//Debug.Log("---NON carico da content " + sectionLoad.contents[contToLoadInd].name);

					}

				}

			}



		}
		catch(Exception e) {

			Debug.Log("AHI+++++++++++++++++++++++++++++++++++++++");
			Debug.Log(e.ToString());
			
		}

	}

	void setConfigurationContent(ref InformativeContent contentToSet, InformativeContent contentLoad) {

		int len = 0;

		if (contentToSet.subContents != null) {

			if(contentToSet.subContents.Length > 0) {

				len = contentToSet.subContents.Length;

				for(int i=0; i<len; i++) {

					contentToSet.subContents[i].subContentViewingTimer = contentLoad.subContents[i].subContentViewingTimer;

					contentToSet.subContents[i].subContentViewsCounter = contentLoad.subContents[i].subContentViewsCounter;

				}

			}

		}


		contentToSet.shownWhenUnlocked = contentLoad.shownWhenUnlocked;

		contentToSet.lockedContent = contentLoad.lockedContent;

		contentToSet.contentViewingTimer = contentLoad.contentViewingTimer;

		contentToSet.contentViewsCounter = contentLoad.contentViewsCounter;

		//----------

		contentToSet.questionAnswered = contentLoad.questionAnswered;
		
		contentToSet.wrongTrials = contentLoad.wrongTrials;
	}
	
	public static InfoSectionContainer Load(string path)
	{
		try{
			var serializer = new XmlSerializer(typeof(InfoSectionContainer));
			using(var stream = new FileStream(path, FileMode.Open))
			{
				return serializer.Deserialize(stream) as InfoSectionContainer;
			}
		}
		catch(XmlException e) {

			throw e;
			
		}
		catch(Exception e) {
			
			throw e;
			
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static InfoSectionContainer LoadFromText(string text) 
	{
		try {
			var serializer = new XmlSerializer(typeof(InfoSectionContainer));
			return serializer.Deserialize(new StringReader(text)) as InfoSectionContainer;
		}
		catch(XmlException e) {

			throw e;

		}
		catch(Exception e) {
			
			throw e;
			
		}

	}
}



[System.Serializable]
public class InformativeSection {

	[SerializeField]
	public string title;

	//TODO: parametro temporaneo
	[SerializeField]
	public int levelN;

	[SerializeField]
	public infoContentType contentType;

	[SerializeField]
	public InformativeContent []contents;

	[HideInInspector]
	public int activeContent;

	//serve per la gestione del menù
	//per aprire o meno le sezioni
	//viene settato a true la prima volta che un suo collezionabile viene sbloccato
	[SerializeField]
	public bool lockedSection = false;

	//serve per capire se si è finito un livello
	//viene gestito a fine e inizio livello da UnlockedLevelControl
	[SerializeField]
	public bool ended = false;


}


[System.Serializable]
public class InformativeContent {

	[SerializeField]
	public string name;

	[HideInInspector]
	[SerializeField]
	public float contentViewingTimer;

	[HideInInspector]
	[SerializeField]
	public int contentViewsCounter;

	[HideInInspector]
	public int activeSubContentIndex = 0;

	[SerializeField]
	public SubContent []subContents;

	[XmlIgnoreAttribute]
	[SerializeField]
	public Sprite iconUnlock;
	
	[XmlIgnoreAttribute]
	[SerializeField]
	public Sprite iconLock;

	[XmlIgnoreAttribute]
	[SerializeField]
	public GameObject unlockerObject;

	[HideInInspector]
	[SerializeField]
	public bool shownWhenUnlocked;
	
	[SerializeField]
	public bool lockedContent = false;


	//--------

	[XmlIgnoreAttribute]
	public GameObject questionObject;

	public bool questionAnswered = false;

	public int wrongTrials = 0;



}


[System.Serializable]
public class SubContent {

	[SerializeField]
	public string name;

	[SerializeField]
	public bool usableForLoading = false;

	[XmlIgnoreAttribute]
	[SerializeField]
	public Sprite image;

	[XmlIgnoreAttribute]
	[SerializeField]
	public TextAsset infoText;

	[HideInInspector]
	[SerializeField]
	public float subContentViewingTimer;

	[HideInInspector]
	[SerializeField]
	public int subContentViewsCounter;

}