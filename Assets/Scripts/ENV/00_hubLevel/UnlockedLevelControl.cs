using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class UnlockedLevelControl : MonoBehaviour {

	[System.Serializable]
	public class UnlockedLevelInfo
	{
		public int levelNumber;
		public bool wasUnlocked = false;
		public bool isUnlocked = false;
	}

	[System.Serializable]
	public class UnlockedLevelElements
	{
		public int levelNumber;
		public UnlockedLevelInfo unlockedLevelInfo;
		public GameObject externalDoor;
		public GameObject inputDoor;
		public bool needToUnlock;
	}

	[SerializeField]
	UnlockedLevelElements[] unlockedLevelElements;
	UnlockedLevelInfo[] unlockedLevelInfo;
	//InformativeSection contentSection;
	public string savedDoorFile = "savedDoor";
	public string savedFileExtention = ".dat";
	int savedInputFileIndex = 0;
	public int indexToLoad = 0;

	void Start () {
		//carico i dati salvati

		//setto i livelli che ho sbloccato nel frattempo come sbloccati
		setUnlocked();

		//controllo se c'è qualche differenza tra quelli salvati e quelli attuali, così da avviare le necessarie animazioni

		//setto quelli salvati come gli attuali
		setWasUnlocked();
	}

	void Update () {
	
	}

	void setUnlocked()
	{
		for (int i = 0; i < unlockedLevelElements.Length; i++)
		{
			if (unlockedLevelElements[i] != null)
			{
				InformativeSection contentSection = GeneralFinder.informativeManager.getActualCollectiblesSection(i+1);
				if (contentSection != null && !contentSection.locked)
					unlockedLevelElements[i].unlockedLevelInfo.isUnlocked = true;
			}
		}
	}

	void setWasUnlocked()
	{
		for (int i = 0; i < unlockedLevelElements.Length; i++)
		{
			if (unlockedLevelElements[i] != null)
			{
				InformativeSection contentSection = GeneralFinder.informativeManager.getActualCollectiblesSection(i+1);
				if (contentSection != null && !contentSection.locked)
					unlockedLevelElements[i].unlockedLevelInfo.wasUnlocked = true;
			}
		}
	}

	void OnDestroy()
	{

	}

	void saveInfo()
	{

	}

	void loadInfo()
	{

	}

	/*
	void save()
	{
		unlockedLevelInfo = new UnlockedLevelInfo[unlockedLevelElements.Length];
		for (int i = 0; i < unlockedLevelInfo.Length; i++)
		{
			if (unlockedLevelElements[i] != null && unlockedLevelElements[i].unlockedLevelInfo != null)
				unlockedLevelInfo[i] = unlockedLevelElements[i].unlockedLevelInfo;
		}

		BinaryFormatter bf = new BinaryFormatter ();
		
		FileStream file = File.Create (Application.persistentDataPath + "/" +savedDoorFile + savedInputFileIndex+ savedFileExtention);
		bf.Serialize (file, changedButtonsList);
		file.Close ();
	}
	
	void load()
	{
		if (File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedButtonFile + indexToLoad + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedButtonFile + indexToLoad + savedFileExtention, FileMode.Open);
			
			changedButtonsList = (List<ChangedButtons>)bf.Deserialize(file);
		}
		
		if (File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedAxisFile + indexToLoad + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file02 = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedAxisFile + indexToLoad + savedFileExtention, FileMode.Open);
			
			changedAxisList = (List<ChangedAxis>)bf.Deserialize(file02);
		}
		
		if (Input.mousePresent && File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedMouseFile + indexToLoad + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file03 = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedMouseFile + indexToLoad + savedFileExtention, FileMode.Open);
			
			changedMouseList = (List<ChangedMouse>)bf.Deserialize(file03);
		}
		
		if (File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedObjectsFile + savedInputFileIndex+ savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file04 = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedObjectsFile + savedInputFileIndex+ savedFileExtention, FileMode.Open);
			
			changedObjectsList = (List<ChangedObjects>)bf.Deserialize(file04);
		}
	}
	*/
}
