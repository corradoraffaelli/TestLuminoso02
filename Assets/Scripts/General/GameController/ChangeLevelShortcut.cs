using UnityEngine;
using System.Collections;

public class ChangeLevelShortcut : MonoBehaviour {

	public string[] levels;

	void Update () {
		if (levels != null && levels.Length != 0)
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0) && levels[0] != null && levels[0] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[0]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1) && levels[1] != null && levels[1] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[1]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2) && levels[2] != null && levels[2] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[2]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3) && levels[3] != null && levels[3] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[3]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4) && levels[4] != null && levels[4] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[4]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5) && levels[5] != null && levels[5] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[5]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6) && levels[6] != null && levels[6] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[6]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7) && levels[7] != null && levels[7] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[7]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8) && levels[8] != null && levels[8] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[8]));
			}
				
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha9) && levels[9] != null && levels[9] != "")
			{
				automaticSave();
				StartCoroutine(changeScene(levels[9]));
			}	
		}

		//shortcut uccisione player
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
		{
			GeneralFinder.player.SendMessage("c_instantKill");
		}

		//shortcut salvataggio
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
		{
			saveInformativeManager();
		}

		//shortcut salvataggio TestUtils
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
		{
			saveAnanlyzeInfo();
		}

		//shortcut salvataggio Hub
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.B))
		{
			saveHubInfos();
		}
	}

	void saveHubInfos()
	{
		if (GeneralFinder.unlockedLevelControl != null)
		{
			GeneralFinder.unlockedLevelControl.saveInfo();
		}
		
		if (GeneralFinder.hubLadderControl != null)
		{
			GeneralFinder.hubLadderControl.saveInfo();
		}
		
		if (GeneralFinder.hubLanternControl != null)
		{
			GeneralFinder.hubLanternControl.saveInfo();
		}
	}

	void saveAnanlyzeInfo()
	{
		GeneralFinder.testingController.GetComponent<ZoneAnalyzer>().saveInfos();
		GeneralFinder.testingController.GetComponent<SpikeAnalyzer>().saveInfos();
		GeneralFinder.testingController.GetComponent<HintAnalyzer>().saveInfos();
	}

	void saveInformativeManager()
	{
		GeneralFinder.informativeManager.c_saveData();
	}

	void automaticSave()
	{
		bool inHub = false;

		if (levels != null && levels.Length != 0 && levels[0] == Application.loadedLevelName)
			inHub = true;

		if (!inHub)
		{
			saveInformativeManager();
			saveAnanlyzeInfo();
		}
		else
		{
			saveHubInfos();
		}

		saveInput();
	}

	IEnumerator changeScene(string levelName)
	{
		GeneralFinder.playerMovements.enabled = false;
		yield return new WaitForSeconds(2.0f);

		Application.LoadLevel(levelName);
	}

	void saveInput()
	{
		if (GeneralFinder.inputKeeper.loadSaveState == InputKeeper.LoadSaveState.Save)
			GeneralFinder.inputKeeper.save();
	}
}
