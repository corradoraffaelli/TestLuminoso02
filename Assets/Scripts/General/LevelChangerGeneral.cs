using UnityEngine;
using System.Collections;

public class LevelChangerGeneral : MonoBehaviour {

	int loadProgress = 0;

	public GameObject blackScreen;
	public GameObject text;

	UnityEngine.UI.Text textUI;

	int progress;

	public GameObject[] canvasToDisable;

	void Start () {
		if (text != null && blackScreen != null) {
			blackScreen.SetActive(false);
			text.SetActive(false);

			textUI = text.GetComponent<UnityEngine.UI.Text>();
		}
	
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Z))
			loadLevel ("00_Level_01#7");
	}

	IEnumerator DisplayLoadingScreen(string level)
	{
		if (blackScreen != null && text != null) {
			blackScreen.SetActive (true);
			text.SetActive (true);

			for (int i = 0; i < canvasToDisable.Length; i++)
			{
				if (canvasToDisable[i] != null)
				{
					canvasToDisable[i].SetActive(false);
				}
			}

			AsyncOperation async = Application.LoadLevelAsync(level);

			while (!async.isDone)
			{
				progress = (int)(async.progress * 100);
				Debug.Log (progress);

				textUI.text = progress.ToString();

				yield return null;

			}

		}
	}

	public void loadLevel(string levelToLoad)
	{
		StartCoroutine (DisplayLoadingScreen (levelToLoad));
	}
}
