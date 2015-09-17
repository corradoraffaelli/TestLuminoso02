using UnityEngine;
using System.Collections;

public class LevelChangerGeneral : MonoBehaviour {

	int loadProgress = 0;
	float loadProgressFloat = 0.0f;

	public GameObject blackScreen;
	public GameObject text;
	public GameObject bar;
	public GameObject loadingText;

	public Sprite[] images;
	public int randomNum = 0;

	UnityEngine.UI.Text textUI;
	RectTransform barTransform;
	UnityEngine.UI.Image screenImage;

	//int progress;

	public GameObject[] canvasToDisable;

	void Start () {
		if (text != null && blackScreen != null && bar != null && loadingText!= null) {
			textUI = text.GetComponent<UnityEngine.UI.Text>();
			barTransform = bar.GetComponent<RectTransform>();
			screenImage = blackScreen.GetComponent<UnityEngine.UI.Image>();

			barTransform.localScale = new Vector3(0.0f, 1.0f, 1.0f);

			blackScreen.SetActive(false);
			text.SetActive(false);
			bar.SetActive(false);
			loadingText.SetActive(false);

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
			blackScreen.SetActive (true);
			text.SetActive (true);
			bar.SetActive(true);
			loadingText.SetActive(true);

			if (screenImage != null && images[randomNum] != null)
			{
				screenImage.sprite = images[randomNum];
				Debug.Log ("immagini");
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


			loadProgressFloat = 1.0f;
			loadProgress = 100;
			
			textUI.text = loadProgress.ToString();
			
			barTransform.localScale = new Vector3(loadProgressFloat, 1.0f, 1.0f);

			yield return new WaitForSeconds(1.0f);

			async.allowSceneActivation = true;

			while (!async.isDone)
			{
				yield return null;
			}

		}
	}

	public void loadLevel(string levelToLoad)
	{
		StartCoroutine (DisplayLoadingScreen (levelToLoad));
	}
}
