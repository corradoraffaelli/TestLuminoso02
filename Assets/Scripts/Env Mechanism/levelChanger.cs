using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class levelChanger : MonoBehaviour {

	public bool inputDoor = false;
	public bool reloadThisLevel = true;
	//public string sceneName;
	public int sceneNumber;
	public int actualLevelNumber = 1;
	public int levelToUnlock = 2;

	[HideInInspector]
	public string savedLevelFile = "savedComingLevel";
	[HideInInspector]
	public string savedFileExtention = ".dat";

	LevelChangerGeneral levelChangerGeneral;

	InteragibileObject interagibileObject;

	void Start()
	{
		GameObject levelChanger = GameObject.FindGameObjectWithTag ("CanvasLoadLevel");
		if (levelChanger != null)
			levelChangerGeneral = levelChanger.GetComponent<LevelChangerGeneral> ();

		interagibileObject = GetComponent<InteragibileObject> ();
	}

	void unlockLevel()
	{
		if (GeneralFinder.informativeManager.getActualCollectiblesSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualCollectiblesSection(levelToUnlock).lockedSection = false;
		if (GeneralFinder.informativeManager.getActualFragmentSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualFragmentSection(levelToUnlock).lockedSection = false;
		if (GeneralFinder.informativeManager.getActualFunFactsSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualFunFactsSection(levelToUnlock).lockedSection = false;
	}

	public void changeScene()
	{
		if (interagibileObject != null)
			interagibileObject.enabled = false;

		if (levelChangerGeneral != null) {
			//if (reloadThisLevel)
			//	levelChangerGeneral.loadLevel ();

			//else if (sceneName != "")
				levelChangerGeneral.loadLevel (sceneNumber);
		} 
		else 
		{
			if (reloadThisLevel)
				Application.LoadLevel (Application.loadedLevel);
			else
				Application.LoadLevel (sceneNumber);
		}
	}

	public void InteractingMethod()
	{
		if (!inputDoor)
		{
			/*
			unlockLevel();
			saveInfo();

			GeneralFinder.informativeManager.c_saveData();
			Debug.Log ("salvo");
			changeScene ();
			*/
			StartCoroutine(ExitDoorBehave());
		}
		else
		{
			StartCoroutine(InputDoorBehave());
		}
	}

	void saveInfo()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		
		FileStream file = File.Create (Application.persistentDataPath + "/" +savedLevelFile + savedFileExtention);
		bf.Serialize (file, actualLevelNumber);
		file.Close ();
	}

	IEnumerator InputDoorBehave() {
		GeneralFinder.playerMovements.enabled = false;
		saveHubInfos();
		saveUseController ();
		//yield return new WaitForSeconds(1.5f);
		yield return new WaitForSeconds(1.0f);
		changeScene ();

	}

	IEnumerator ExitDoorBehave() {
		unlockLevel();
		saveInfo();

		saveTesting();

		saveUseController ();
		
		GeneralFinder.informativeManager.c_saveData();
		Debug.Log ("salvo");

		if (GeneralFinder.inputKeeper.loadSaveState == InputKeeper.LoadSaveState.Save)
			saveInput();
		
		GeneralFinder.playerMovements.enabled = false;
		//yield return new WaitForSeconds(2.0f);
		yield return new WaitForSeconds(1.0f);
		
		changeScene ();
	}

	//funzione per salvare dati di testing
	void saveTesting()
	{
		GeneralFinder.testingController.GetComponent<ZoneAnalyzer>().saveInfos();
		GeneralFinder.testingController.GetComponent<SpikeAnalyzer>().saveInfos();
		GeneralFinder.testingController.GetComponent<HintAnalyzer>().saveInfos();
	}

	void saveUseController()
	{
		GeneralFinder.cursorHandler.saveInfo ();
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

	void saveInput()
	{
		GeneralFinder.inputKeeper.save();
	}
}
