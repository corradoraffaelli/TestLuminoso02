using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class setStartLevelButtons : MonoBehaviour {

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

			button.onClick.AddListener(() => startLevel(sceneName));

			textButton.text = sceneName;

			tempButt.name = sceneName + " Button";

			tempButt.transform.SetAsFirstSibling();

		}

		setExitButton ();



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
		
		textButton.text = "Exit Game";

	}

	public void startLevel(string nameScene) {

		Application.LoadLevel (nameScene);

	}

	public void quitGame() {

		Application.Quit ();

	}

	// Update is called once per frame
	void Update () {
		
	}
}
