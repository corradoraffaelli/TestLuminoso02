using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InformativeManager : MonoBehaviour {

	#region VARIABLES

	#region PUBLICVARIABLES

	public GameObject canvasMenu;
	public GameObject canvasInformative;
	public GameObject canvasIntro;

	public GameObject canvasPlayingUI;

	[SerializeField]
	InformativeSection []sections;

	[SerializeField]
	int activeSection;

	#endregion PUBLICVARIABLES

	#region PRIVATEVARIABLES

	PlayingUI playingUI;
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

	}

	#region STARTMETHODS

	void initializeReferences() {

		if (initialized)
			return;

		menuMan = UtilFinder._GetComponentOfGameObjectWithTag<MenuManager> ("Controller");

		if (canvasPlayingUI != null) {
			playingUI = UtilFinder._GetComponent<PlayingUI> (canvasPlayingUI);
		} 
		else {

			Debug.Log ("ATTENZIONE - canvasPlayingUI non assegnato a InformativeManager");
		
		}

		if (canvasMenu != null) {

			//bool activeState = canvasMenu.activeSelf;



			if (getCanvas ()) {


				canvasMenu.SetActive (true);
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
				
				canvasMenu.SetActive (false);


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
						exitButton.onClick.AddListener(() => { c_activeInformative(false); });
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

		foreach (Transform child in canvasMenu.transform) {
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

	#endregion STARTMETHODS

	//RIEMPIMENTO SEZIONE-----


	/*
	public void showContent() {

		if (activeSection < 0)
			return;

		if (sections [activeSection] == null)
			return;

		fillNavigation (activeSection);

		fillMultimedia (activeSection, sections [activeSection].activeContent);

		fillDetail (activeSection, sections [activeSection].activeContent);




	}
	*/

	#region PRIVATEMETHODS

	void fillNavigation(int sectionN) {

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

	void fillMultimedia(int sectionN, int contentN, int imageN=0) {

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

	void fillDetail(int sectionN, int contentN) {

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

	void activeTempCanvasInformative(bool act) {

		canvasInformative.SetActive (act);

	}

	void Update() {
		
		if (canShowTemporarely && Input.GetKey (KeyCode.I)) {
			
			c_activeInformative (true);
			playingUI.cleanPositionButtonObject (PlayingUI.UIPosition.UpperRight);
			playingUI.cleanPositionGameObjects (PlayingUI.UIPosition.UpperRight);
			
			canShowTemporarely = false;
		}
		
	}

	#endregion PRIVATEMETHODS

	#region CALLBACKS

	public void c_changeSection(bool forward) {

		int nextIndex = 0;

		for (int i=0; i<sections.Length; i++) {

			if (forward) {
				activeSection = (activeSection + 1) % sections.Length;
			} else {
				activeSection = (activeSection - 1) % sections.Length;
			}

			if(!sections[activeSection].locked)
				break;

		}
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
		
		if(!canvasMenu.activeSelf)
			canvasMenu.SetActive(true);
		
		if (act) {
			menuMan.c_switchMenuSection (null, canvasInformative);
		} 
		else {
			menuMan.c_switchMenuSection (canvasInformative, canvasIntro);
		}
	}

	public void c_initializeInformative(){
		
		initializeReferences ();
		
	}

	public void c_canShowNewContent(int sect, int cont, bool unlock = true) {
		
		canShowTemporarely = true;
		
		if (sect >= sections.Length)
			return;
		
		activeSection = sect;
		
		if(cont >= (sections [activeSection].contents.Length ) )
			return;
		
		sections [activeSection].activeContent = cont;
		
		if (unlock) {
			
			sections [activeSection].contents[cont].locked = false;
			
		}
		
		StartCoroutine ("countDownShowNewContent");
	}


	#endregion CALLBACKS

	#region COROUTINES

	IEnumerator countDownShowNewContent() {
		
		yield return new WaitForSeconds (5.0f);
		
		playingUI.cleanPositionButtonObject(PlayingUI.UIPosition.UpperRight);
		playingUI.cleanPositionGameObjects(PlayingUI.UIPosition.UpperRight);
		
		canShowTemporarely = false;
		
	}

	#endregion COROUTINES


	
}




[System.Serializable]
public class InformativeSection {

	[SerializeField]
	public string title;

	[SerializeField]
	public InformativeSet []contents;

	public int activeContent;

	[SerializeField]
	public bool locked = false;

}

[System.Serializable]
public class InformativeSet {

	[SerializeField]
	public string name;

	[SerializeField]
	public Sprite []mainImages;

	[HideInInspector]
	public int mainImageIndex = 0;

	[SerializeField]
	public Sprite iconUnlock;

	[SerializeField]
	public Sprite iconLock;

	[SerializeField]
	public TextAsset infoText;

	[SerializeField]
	public bool locked = false;

}