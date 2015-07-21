using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class setStartLevelButtons : MonoBehaviour {

	public string defPath = "Config/InformativeConf/";
	public string defaultFileName = "InfoFileConf";

	public GameObject buttonMold;

	public string []sceneNamesToPlay;

	// Use this for initialization
	void Start () {
		initializeStartMenuButtons ();
	}

	void initializeStartMenuButtons() {

		foreach (string sceneName in sceneNamesToPlay) {

			GameObject tempButt = (GameObject) Instantiate(buttonMold,Vector3.zero, Quaternion.identity);

			tempButt.transform.SetParent(transform);

			tempButt.transform.localScale = new Vector3(1.0f,1.0f,1.0f);

			tempButt.SetActive(true);

			Button button = tempButt.GetComponent<Button>();
			Text textButton = tempButt.GetComponentInChildren<Text>();



			switch (sceneName) {

				case "00_Level_00" :
					textButton.text = "Nuova Partita";
					button.onClick.AddListener(() => startLevel(sceneName, true));
					break;
				default :
					button.onClick.AddListener(() => startLevel(sceneName));
					break;
			}


			tempButt.name = sceneName + " Button";

			tempButt.transform.SetAsFirstSibling();

		}

		if (isPresentData ()) {

			setContinueButton ();
		
		}
		setExitButton ();



	}

	bool isPresentData() {

		//TODO : mettere or isPresentHubData
		if (isPresentInfo ())
			return true;
		else
			return false;

	}

	bool isPresentInfo() {

		string dirName = Path.GetDirectoryName (defPath);
		
		if (!Directory.Exists (dirName)) {
			
			Directory.CreateDirectory (dirName);
			
			return;
		} else {
			
			string [] files = Directory.GetFiles (defPath);
			
			foreach(string fileName in files) {
				
				if(fileName.Contains(defaultFileName)) {
					
					return true;
					
					
				}
				
			}
			
		}

		return false;

	}

	void setContinueButton () {

		GameObject tempButt = (GameObject) Instantiate(buttonMold,Vector3.zero, Quaternion.identity);
		
		tempButt.name = "Continue Button";
		
		tempButt.transform.SetParent(transform);
		
		tempButt.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
		
		tempButt.SetActive(true);
		
		Button button = tempButt.GetComponent<Button>();
		Text textButton = tempButt.GetComponentInChildren<Text>();
		
		button.onClick.AddListener(() => startLevel("00_Level_00"));
		
		textButton.text = "Continua";

	}

	void setExitButton() {

		GameObject tempButt = (GameObject) Instantiate(buttonMold,Vector3.zero, Quaternion.identity);

		tempButt.name = "Exit Button";
		
		tempButt.transform.SetParent(transform);

		tempButt.transform.localScale = new Vector3(1.0f,1.0f,1.0f);

		tempButt.SetActive(true);
		
		Button button = tempButt.GetComponent<Button>();
		Text textButton = tempButt.GetComponentInChildren<Text>();
		
		button.onClick.AddListener(() => quitGame());
		
		textButton.text = "Esci";

	}

	public void startLevel(string nameScene, bool cleanData=false) {

		if (cleanData) {

			cleanHubData();

			cleanInformativeData();

		}

		Application.LoadLevel (nameScene);

	}

	void cleanHubData() {


	}

	void cleanInformativeData() {

		string dirName = Path.GetDirectoryName (defPath);

		if (!Directory.Exists (dirName)) {
			
			Directory.CreateDirectory (dirName);
			
			return;
		} else {
			
			string [] files = Directory.GetFiles (defPath);

			foreach(string fileName in files) {
				
				if(fileName.Contains(defaultFileName)) {

					File.Delete(fileName);

				
				}
			
			}
		
		}

	}

	public void quitGame() {

		Application.Quit ();

	}

	// Update is called once per frame
	void Update () {
		
	}
}
