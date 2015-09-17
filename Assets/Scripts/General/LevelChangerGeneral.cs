using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelChangerGeneral : MonoBehaviour {

	int loadProgress = 0;
	float loadProgressFloat = 0.0f;

	public GameObject blackScreen;
	public GameObject background;
	public GameObject text;
	public GameObject bar;
	public GameObject loadingText;
	public GameObject textArea;
	public GameObject loadArea;

	public Sprite[] images;
	public int randomNum = 0;

	UnityEngine.UI.Text textUI;
	RectTransform barTransform;
	UnityEngine.UI.Image screenImage;

	public float extraLoadTime = 5.0f;
	
	//int progress;

	public GameObject[] canvasToDisable;

	void Start () {
		if (text != null && blackScreen != null && bar != null && loadingText!= null) {
			textUI = text.GetComponent<UnityEngine.UI.Text>();
			barTransform = bar.GetComponent<RectTransform>();
			screenImage = blackScreen.GetComponent<UnityEngine.UI.Image>();

			barTransform.localScale = new Vector3(0.0f, 1.0f, 1.0f);

			if(loadArea!=null) {
				loadArea.SetActive(false);
			}

			if(background!=null)
				background.SetActive(false);

			blackScreen.SetActive(false);
			text.SetActive(false);
			bar.SetActive(false);
			loadingText.SetActive(false);

			if(textArea!=null)
				textArea.SetActive(false);

			for (int i = 0; i< images.Length; i++)
			{
				if (images[i] != null)
				{
					randomNum = Random.Range(0, images.Length);
					while (images[randomNum] == null)
						randomNum = Random.Range(0, images.Length);
					break;
				}
			}
		}
	
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Z))
			loadLevel ("00_Level_01#7");

	}

	IEnumerator DisplayLoadingScreen(string level)
	{
		if (blackScreen != null && text != null && bar != null) {

			if(loadArea!=null) {
				loadArea.SetActive(true);
			}

			if(background!=null) {
				background.SetActive(true);
			}

			blackScreen.SetActive (true);
			text.SetActive (true);
			bar.SetActive(true);
			loadingText.SetActive(true);

			if(textArea!=null)
				textArea.SetActive(true);

			SubContent subC = GeneralFinder.informativeManager.c_getRandomSubContent ();
			
			if (subC != null) {
				
				if (screenImage != null && subC.image != null)
				{
					screenImage.sprite = subC.image;

					Debug.Log ("immagini sub content");
					if(textArea!=null) {
						Text t = textArea.GetComponentInChildren<Text>();
						t.text = subC.infoText.text;
					}
				}
				
			}
			else {
				
				if (screenImage != null && images[randomNum] != null)
				{
					screenImage.sprite = images[randomNum];
					if(textArea!=null)
						textArea.SetActive(false);
					Debug.Log ("immagini");
				}
				
			}


				

			for (int i = 0; i < canvasToDisable.Length; i++)
			{
				if (canvasToDisable[i] != null)
				{
					canvasToDisable[i].SetActive(false);
				}
			}

			AsyncOperation async = Application.LoadLevelAsync(level);
			async.allowSceneActivation = false;

			while (loadProgressFloat<0.85f)
			{
				loadProgressFloat = async.progress;
				loadProgress = (int)(loadProgressFloat * 100);
				//Debug.Log (loadProgress);

				textUI.text = loadProgress.ToString()+"%";
				barTransform.localScale = new Vector3(loadProgressFloat, 1.0f, 1.0f);

				yield return null;
			}

			yield return new WaitForSeconds(extraLoadTime);

			loadProgressFloat = 1.0f;
			loadProgress = 100;
			
			textUI.text = loadProgress.ToString();
			
			barTransform.localScale = new Vector3(loadProgressFloat, 1.0f, 1.0f);

			async.allowSceneActivation = true;

			while (!async.isDone)
			{
				yield return null;
			}

			//attendi utente che preme continua

		}
	}
	


	public void loadLevel(string levelToLoad)
	{
		StartCoroutine (DisplayLoadingScreen (levelToLoad));
	}
}
