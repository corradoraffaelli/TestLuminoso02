using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class levelChanger : MonoBehaviour {

	public bool inputDoor = false;
	public bool reloadThisLevel = true;
	public string sceneName;
	public int actualLevelNumber = 1;
	public int levelToUnlock = 2;

	[HideInInspector]
	public string savedLevelFile = "savedComingLevel";
	[HideInInspector]
	public string savedFileExtention = ".dat";

	void unlockLevel()
	{
		if (GeneralFinder.informativeManager.getActualCollectiblesSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualCollectiblesSection(levelToUnlock).locked = false;
		if (GeneralFinder.informativeManager.getActualFragmentSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualFragmentSection(levelToUnlock).locked = false;
		if (GeneralFinder.informativeManager.getActualFunFactsSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualFunFactsSection(levelToUnlock).locked = false;
	}

	public void changeScene()
	{
		if (reloadThisLevel)
			Application.LoadLevel (Application.loadedLevel);
		else
			if (sceneName != "")
				Application.LoadLevel (sceneName);
	}

	public void InteractingMethod()
	{
		if (!inputDoor)
		{
			unlockLevel();
			saveInfo();

			GeneralFinder.informativeManager.c_saveData();
			Debug.Log ("salvo");
			changeScene ();
		}
		else
		{
			StartCoroutine(Example());
		}
	}

	void saveInfo()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		
		FileStream file = File.Create (Application.persistentDataPath + "/" +savedLevelFile + savedFileExtention);
		bf.Serialize (file, actualLevelNumber);
		file.Close ();
	}

	IEnumerator Example() {
		yield return new WaitForSeconds(1);
		changeScene ();
	}
}
