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

public class InformativeManager : MonoBehaviour {

	#region VARIABLES

	#region PUBLICVARIABLES

	//public GameObject canvasMenu;
	GameObject canvasInformative;
	GameObject canvasIntro;

	public bool loadDefaultConf;

	[SerializeField]
	public InformativeSection []sections;

	[SerializeField]
	int activeSection;

	public int actualLevelNumber = -1;

	#endregion PUBLICVARIABLES

	#region PRIVATEVARIABLES
	
	MenuManager menuMan;

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

	GameObject detailSection;
	Text detail;

	Button exitButton;

	bool canShowTemporarely = false;

	bool initialized = false;
	

	#endregion PRIVATEVARIABLES

	#endregion VARIABLES



	void Start () {

		initializeReferences ();

		InfoSectionContainer.tryLoadInformativeManagerConf (ref sections);

		setUnlockerOfThisLevel ();

		if (loadDefaultConf) {
			//loadInformativeManagerConf ();
			//saveInformativeManagerConf ();
		}
		else {


		}

		/*
		InfoSectionContainer infocon = new InfoSectionContainer ();

		TextAsset pi = Resources.Load("InformativeFileConf") as TextAsset;

		//infocon = InfoSectionContainer.Load("/Users/dariorandazzo/Unity Projects/Magic 02/Assets/TextAssets/provainfo.xml");
		infocon = InfoSectionContainer.LoadFromText (pi.text);

		infocon.setConfiguration (ref sections);
		*/
		//infocon.sections = sections;

		//infocon.Save ("/Users/dariorandazzo/Unity Projects/Magic 02/Assets/TextAssets/provainfo.xml");

	}

	#region PRIVATEMETHODS

	#region STARTMETHODS

	void setUnlockerOfThisLevel() {
		
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
							if(!conte.locked) {
								
								conte.unlockerObject.SetActive(false);
								
							}
							else {
								
								conte.unlockerObject.SendMessage("c_setSectionInt", sectionIndex);
								conte.unlockerObject.SendMessage("c_setContentInt", contentIndex);
								
							}
							
							
						}

						contentIndex++;
					}
					break;

				case infoContentType.FunFacts :
					foreach (InformativeContent funfact in section.contents) {
						
						if(funfact.unlockerObject!=null) {
							if(!funfact.locked) {
								
								funfact.unlockerObject.SetActive(false);
								
							}
							else {
								
								funfact.unlockerObject.SendMessage("c_setSectionInt", sectionIndex);
								funfact.unlockerObject.SendMessage("c_setContentInt", contentIndex);
								
							}
							
							
							}
							
						
						contentIndex++;
					}
					break;
					
				case infoContentType.Fragments :
					foreach (InformativeContent fragme in section.contents) {
						
						if(fragme.unlockerObject!=null) {
							if(!fragme.locked) {
								
								fragme.unlockerObject.SetActive(false);
								
							}
							else {
								
								fragme.unlockerObject.SendMessage("c_setSectionInt", sectionIndex);
								fragme.unlockerObject.SendMessage("c_setFragmentInt", contentIndex);
								
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

	void initializeReferences() {

		if (initialized)
			return;

		menuMan = UtilFinder._GetComponentOfGameObjectWithTag<MenuManager> ("Controller");


		if (GeneralFinder.canvasMenu != null) {

			//bool activeState = canvasMenu.activeSelf;



			if (getCanvas ()) {


				GeneralFinder.canvasMenu.SetActive (true);
				activeTempCanvasInformative(true);

				if(getSections()) {

					getNavigationComponents ();
					
					getMultimediaComponents ();
					
					getDetailComponents ();

					fillDetail (activeSection, 0);
					
					fillMultimedia (activeSection, 0);
					
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
								multimediaButtons[0].onClick.AddListener(() => { c_changeMultimedia(false); });

							}
							
							if(newbie.name=="Right") {

								multimediaButtons[1] = newbie.GetComponent<Button>();
								multimediaButtons[1].onClick.AddListener(() => { c_changeMultimedia(true); });

							}

						}

					}
				}
				//multimediaButtons

			}

			if(child.name=="LateralMedia") {

				foreach (Transform nephew in child.transform) {
					
					if(nephew.name=="High") {

						exitButton = nephew.GetComponentInChildren<Button>();
						exitButton.onClick.AddListener(() => { GeneralFinder.menuManager.c_switchMenuSection (canvasInformative, canvasIntro); });
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

		return true;

	}

	#endregion GETGRAPHICREF

	#endregion STARTMETHODS






	void activeTempCanvasInformative(bool act) {

		canvasInformative.SetActive (act);

	}

	void Update() {
		
		if (canShowTemporarely && Input.GetKey (KeyCode.I)) {

			GeneralFinder.menuManager.c_enableMenu(true, canvasInformative);

			GeneralFinder.playingUI.cleanPositionButtonObject (PlayingUI.UIPosition.UpperRight);
			GeneralFinder.playingUI.cleanPositionGameObjects (PlayingUI.UIPosition.UpperRight);
			
			canShowTemporarely = false;
		}
		
	}

	#endregion PRIVATEMETHODS



	#region CALLBACKS

	
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
			
			if(!sections [sectionN].contents[index].locked) {
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
	
	public void fillMultimedia(int sectionN = -3, int contentN = -1, int imageN=0) {

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
		
		if (!sections [sectionN].contents [contentN].locked && sections [sectionN].contents [contentN].mainImages != null) {
			
			if(sections [sectionN].contents [contentN].mainImages[imageN] != null) {
				multimedia.sprite = sections [sectionN].contents [contentN].mainImages [imageN];
			}
			
		}
	}
	
	public void fillDetail(int sectionN = -2, int contentN = -1) {

		if (sectionN == -2) {
			
			sectionN = activeSection;
			
		}
		
		if (contentN == -1) {
			
			contentN = sections[sectionN].activeContent;
			
		}

		if (sections [sectionN].contents [contentN] == null){
			Debug.Log ("ATTENZIONE - contenuto nullo");
			return;
		}
		
		if (!sections [sectionN].contents [contentN].locked) {
			if(sections [sectionN].contents [contentN].infoText != null) {
				
				detail.text = sections [sectionN].contents [contentN].infoText.text;
				detail.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0.0f, 0.0f);
				//TODO: adjust lenght of rect based on text lenght
				detail.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0.0f, -400.0f);
			}
			else {
				detail.text = "";
			}
		}
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

			if(!sections[tempActiveSection].locked && sections[tempActiveSection].contentType != infoContentType.Fragments)
				break;

		}

		if (tempActiveSection >= 0) 
			activeSection = tempActiveSection;

		fillNavigation (activeSection);

		fillMultimedia (activeSection, sections [activeSection].activeContent);

		fillDetail (activeSection, sections [activeSection].activeContent);

	}

	public void c_changeMultimedia(bool forward) {

		int nextIndex = 0;

		int activeContent = sections [activeSection].activeContent;
		int activeImage = sections [activeSection].contents [activeContent].mainImageIndex;
		//int lenghtImages = sections [activeSection].contents [activeContent].mainImages.Lenght;

		int lenImages = sections[activeSection].contents[activeContent].mainImages.Length;

		//Debug.Log ("len imgs" + lenImages);

		for (int i=0; i< lenImages; i++) {

			Sprite sp = sections[activeSection].contents[activeContent].mainImages[i];

			if(sp==null) {
				Debug.Log ("ATTENZIONE - alcune sprite delle immagini multimedia non sono state assegnate, section : " + activeSection + " content : " + activeContent);
				lenImages = i;
				break;
			}

		}

		if (forward) {
			activeImage = (activeImage+1)%lenImages;
		} 
		else {
			activeImage = (activeImage-1)%lenImages;
		}

		sections [activeSection].contents [activeContent].mainImageIndex = activeImage;

		fillMultimedia (activeSection, sections [activeSection].activeContent, activeImage);

	}

	public void c_changeContent(int contentN) {
		//Debug.Log ("cambio a " + contentN);
		fillMultimedia(activeSection, contentN);
		fillDetail (activeSection, contentN);
		sections [activeSection].activeContent = contentN;
	}

	public void c_activeInformative(bool act) {
		
		if(!GeneralFinder.canvasMenu.activeSelf)
			GeneralFinder.canvasMenu.SetActive(true);
		
		if (act) {
			menuMan.c_enableMenu(true);
			menuMan.c_switchMenuSection (canvasIntro, canvasInformative);
		} 
		else {
			menuMan.c_switchMenuSection (canvasInformative, canvasIntro);
		}
	}

	public void c_initializeInformative(){
		
		initializeReferences ();
		
	}

	//UNLOCK CONTENT
	public void c_canShowNewContent(int sect, int cont) {
		
		canShowTemporarely = true;
		
		if (sect >= sections.Length) {
			Debug.Log("ATTENZIONE - section oltre la size durante lo sblocco del contenuto");
			return;
		}

		if (sections [sect].contentType != infoContentType.Collectibles || sections [sect].contentType != infoContentType.FunFacts) {
			Debug.Log("ATTENZIONE - section NON di tipo collectibles o fun facts");
			return;
			
		}
		
		activeSection = sect;
		
		if (cont >= (sections [activeSection].contents.Length)) {
			Debug.Log("ATTENZIONE - contenuto oltre la size durante lo sblocco del contenuto");
			return;
		}

		sections [activeSection].locked = false;
		sections [activeSection].activeContent = cont;


		sections [activeSection].contents[cont].locked = false;

		//TODO: cambiare?

		GeneralFinder.unlockableContentUI.unlockContent (sections[sect].title, sections[sect].contents[cont].name);

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
		
		if (fragm >= (sections [activeSection].contents.Length)) {
			Debug.Log("ATTENZIONE - numero del fragment oltre la size durante lo sblocco del contenuto");
			return;
		}
		
		sections [activeSection].locked = false;
		
		sections [activeSection].contents[fragm].locked = false;

		//TODO : cambiare
		GeneralFinder.unlockableContentUI.unlockFragment (sections [sect].title, sections [sect].contents [fragm].name);

	}
	

	public void c_saveData() {
		Debug.Log ("ciaone");
		InfoSectionContainer.saveInformativeManagerConf (sections, true);
		
	}

	public void c_saveData1() {
		Debug.Log ("ciaone");
		InfoSectionContainer.saveInformativeManagerConf (sections, false);

	}


	public InformativeSection getActualCollectiblesSection (int levelN = -1) {

		int levelNumber;

		if (levelN >= 0) {

			levelNumber = levelN;

		} else {

			levelNumber = actualLevelNumber;

		}

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

	#endregion CALLBACKS

	#region COROUTINES

	IEnumerator countDownShowNewContent() {
		
		yield return new WaitForSeconds (5.0f);
		
		GeneralFinder.playingUI.cleanPositionButtonObject(PlayingUI.UIPosition.UpperRight);
		GeneralFinder.playingUI.cleanPositionGameObjects(PlayingUI.UIPosition.UpperRight);
		
		canShowTemporarely = false;
		
	}

	#endregion COROUTINES


	
}

[XmlRoot("InfoSectionCollection")]
public class InfoSectionContainer
{

	static string defaultPath = "Config/InformativeConf/";
	static string defaultFileName = "InfoFileConf";

	[XmlArray("Sections"),XmlArrayItem("Section")]
	public InformativeSection[] sections;

	public static void saveInformativeManagerConf(InformativeSection []_sections, bool defaultName) {
		
		InfoSectionContainer infoCon = new InfoSectionContainer ();
		infoCon.sections = _sections;

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
		catch(System.Exception e) {
			
			throw e;
			
		}
	}

	public void Save(string path)
	{
		Debug.Log ("ciaone1");
		try {
			string dirName = Path.GetDirectoryName (path);
			string fileName = Path.GetFileName (path);

			Debug.Log ("path" + dirName + " e file " + fileName);

			if (!Directory.Exists (dirName)) {
				Debug.Log ("ciaone2");
				Directory.CreateDirectory(dirName);
				
			}
			Debug.Log ("ciaone3");
			var serializer = new XmlSerializer(typeof(InfoSectionContainer));
			using(var stream = new FileStream(path, FileMode.Create))
			{
				Debug.Log ("ciaone4");
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
		catch(System.Exception e) {
			Debug.Log ("ciaoneex2");
			throw e;
			
		}

	}

	public static void tryLoadInformativeManagerConf(ref InformativeSection []_sections) {
		
		
		if (Directory.Exists ("Config/informativeConf/")) {
			
			try {
				
				string []files = Directory.GetFiles("Config/informativeConf/");
				
				foreach(string filePath in files) {
					
					string fileName = Path.GetFileName(filePath);
					string fileExt = Path.GetExtension(filePath);
					
					#if _DEBUG
					Debug.Log("file name " + fileName + " fileext " + fileExt);
					#endif

					//TODO: gestire meglio scelta file da caricare
					//magari try catch dentro foreach così che passa al prossimo

					if(fileName.Contains("InformativeFileConf") && fileExt==".xml") {
						
						InfoSectionContainer.loadInformativeManagerConf (ref _sections, filePath);
						//Debug.Log("CARICAMENTO RIUSCITO");
						return;
					}
					
				}
				
				
			}
			catch(XmlException e) {

				Debug.Log(e.ToString());

				InfoSectionContainer.loadInformativeManagerConf(ref _sections);
				
				return;
				
			}
			catch(System.Exception e) {

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
			TextAsset pi = Resources.Load("InformativeFileConf") as TextAsset;
			
			infocon = InfoSectionContainer.LoadFromText (pi.text);
			
			infocon.setConfiguration (ref _sections);
			
		}
		else {
			infocon = InfoSectionContainer.Load(_path);
			
			infocon.setConfiguration (ref _sections);
		}
		
		
		
		
		
		//infocon = InfoSectionContainer.Load("/Users/dariorandazzo/Unity Projects/Magic 02/Assets/TextAssets/provainfo.xml");
		
	}

	public void setConfiguration(ref InformativeSection[] sectionsToSet) {

		foreach (InformativeSection sectionToSet in sectionsToSet) {

			foreach(InformativeSection loadedSection in sections) {

				if(loadedSection.title==sectionToSet.title && loadedSection.levelN == sectionToSet.levelN) {

					try {

						foreach(InformativeContent contentToSet in sectionToSet.contents) {

							foreach(InformativeContent loadedContent in loadedSection.contents) {

								if(contentToSet.name == loadedContent.name) {

									contentToSet.locked = loadedContent.locked;

									break;
								}

							}

						}

					}
					catch(System.Exception e) {
						
						
					}


					sectionToSet.locked = loadedSection.locked;

					break;

				}

			}

		}


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
		catch(System.Exception e) {
			
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
		catch(System.Exception e) {
			
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

	[SerializeField]
	public bool locked = false;

}


[System.Serializable]
public class InformativeContent {

	[SerializeField]
	public string name;

	[XmlIgnoreAttribute]
	[SerializeField]
	public Sprite []mainImages;

	[HideInInspector]
	public int mainImageIndex = 0;

	[XmlIgnoreAttribute]
	[SerializeField]
	public Sprite iconUnlock;

	[XmlIgnoreAttribute]
	[SerializeField]
	public Sprite iconLock;

	[XmlIgnoreAttribute]
	[SerializeField]
	public TextAsset infoText;

	[XmlIgnoreAttribute]
	[SerializeField]
	public GameObject unlockerObject;

	[SerializeField]
	public bool locked = false;

}