using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelChangerGeneral : MonoBehaviour {

	int loadProgress = 0;
	float loadProgressFloat = 0.0f;

	public GameObject blackScreen;
	//public GameObject background;
	public GameObject textNumber;
	public GameObject bar;
	public GameObject loadingText;
	public GameObject textArea;
	//public GameObject loadArea;
	public GameObject loadCircle;
	public GameObject shadow;
	public GameObject pressButton;

	public Sprite[] images;
	public int randomNum = 0;

	UnityEngine.UI.Text textUI;
	RectTransform barTransform;
	UnityEngine.UI.Image screenImage;

	public float extraLoadTime = 5.0f;

	public float circleSpeed = 5.0f;

	public GameObject[] canvasToDisable;

	bool loadEnded = false;
	bool loadEndedOLD = false;
	AsyncOperation async;

	void Start () {

		if (textNumber != null)
		{
			textUI = textNumber.GetComponent<UnityEngine.UI.Text>();
			textNumber.SetActive(false);
		}

		if (blackScreen != null) {
			blackScreen.SetActive(false);
			screenImage = blackScreen.GetComponent<UnityEngine.UI.Image>();
		}
			
		if (bar != null)
		{
			barTransform = bar.GetComponent<RectTransform>();
			barTransform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
		}

		if (loadingText != null) {
			loadingText.SetActive (false);
		}


		//if(loadArea!=null) {
		//	loadArea.SetActive(false);
		//}

		if (pressButton != null)
			pressButton.SetActive (false);
		//if(background!=null)
		//	background.SetActive(false);



		//bar.SetActive(false);
		//loadingText.SetActive(false);
		if(shadow!=null) {
			shadow.SetActive(false);
		}
		if(loadCircle!=null) {
			loadCircle.SetActive(false);
		}

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

	void Update()
	{
		if(loadCircle!=null) {
			Vector3 actRot = loadCircle.transform.localEulerAngles;
			float zRot = actRot.z;
			zRot = zRot + -Time.deltaTime*circleSpeed*30.0f;
			Vector3 newRot = new Vector3(actRot.x, actRot.y, zRot);
			loadCircle.transform.localEulerAngles = newRot;
			//loadCircle.SetActive(true);
		}

		if (loadEnded) {
			if (GeneralFinder.inputKeeper.isButtonUp("Interaction"))
			{
				//Debug.Log ("premuto E");
				async.allowSceneActivation = true;
			}
				
		}

		if (loadEnded != loadEndedOLD) {
			if (loadEnded)
			{
				if (pressButton != null)
					pressButton.SetActive (true);
			}
		}

		loadEndedOLD = loadEnded;
	}

	IEnumerator DisplayLoadingScreen(int level)
	{

		//if(loadArea!=null) {
		//	loadArea.SetActive(true);
		//}

		//if(background!=null) {
		//	background.SetActive(true);
		//}
		if (blackScreen != null)
			blackScreen.SetActive (true);
		if (textNumber != null)
			textNumber.SetActive (true);
		//bar.SetActive(true);
		//loadingText.SetActive(true);
		if(shadow!=null) {
			shadow.SetActive(true);
		}
		if(loadCircle!=null) {
			loadCircle.SetActive(true);
		}

		if(textArea!=null)
			textArea.SetActive(true);

		SubContent subC = GeneralFinder.informativeManager.c_getRandomSubContent ();
		
		if (subC != null) {
			
			if (screenImage != null && subC.image != null)
			{
				screenImage.sprite = subC.image;

				Debug.Log ("immagini sub content");
				if(textArea!=null) {
					Text t = textArea.GetComponent<Text>();
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

		//AsyncOperation async = Application.LoadLevelAsync(level);
		async = Application.LoadLevelAsync(level);
		async.allowSceneActivation = false;
		//Debug.Log ("cerco di avviare il livello " + level);
		while (loadProgressFloat<0.85f)
		{
			loadProgressFloat = async.progress;
			loadProgress = (int)(loadProgressFloat * 100);
			//Debug.Log (loadProgress);

			if (textUI != null)
				textUI.text = loadProgress.ToString();
			//barTransform.localScale = new Vector3(loadProgressFloat, 1.0f, 1.0f);

			yield return null;
		}

		yield return new WaitForSeconds(extraLoadTime);

		loadProgressFloat = 1.0f;
		loadProgress = 100;

		if (textUI != null)
			textUI.text = loadProgress.ToString();
		
		//barTransform.localScale = new Vector3(loadProgressFloat, 1.0f, 1.0f);
		
		loadEnded = true;

		loadCircle.SetActive (false);

		textNumber.SetActive (false);

		pressButton.SetActive (true);

		while (!async.isDone)
		//while (async.progress != 1.0f)
		{
			yield return null;
		}

		//attendi utente che preme continua

	}
	


	public void loadLevel(int levelToLoad)
	{
		StartCoroutine (DisplayLoadingScreen (levelToLoad));
	}
}
