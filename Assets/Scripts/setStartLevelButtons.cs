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

			tempButt.SetActive(true);

			tempButt.transform.SetParent(transform);

			Button button = tempButt.GetComponent<Button>();
			Text textButton = tempButt.GetComponentInChildren<Text>();

			button.onClick.AddListener(() => startLevel(sceneName));

			textButton.text = sceneName;

			tempButt.transform.SetAsFirstSibling();

		}



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
