using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class HubPlayerBeginPosition : MonoBehaviour {

	[HideInInspector]
	public string savedLevelFile = "savedComingLevel";
	[HideInInspector]
	public string savedFileExtention = ".dat";
	int comingLevel = 1;
	UnlockedLevelControl unlockedLevelControl;
	Vector3 beginPosition;

	void Start () {
		unlockedLevelControl = GetComponent<UnlockedLevelControl>();
		loadInfo();
		getDoorPosition();
		setPlayerPosition();
	}

	void loadInfo()
	{
		if (File.Exists (Application.persistentDataPath + "/" + savedLevelFile + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/" + savedLevelFile + savedFileExtention, FileMode.Open);
			
			comingLevel = (int)bf.Deserialize(file);
		}
	}

	void getDoorPosition()
	{
		if (unlockedLevelControl != null)
		{
			if (unlockedLevelControl.unlockedLevelElements != null)
			{
				for (int i = 0; i < unlockedLevelControl.unlockedLevelElements.Length; i++)
				{
					if (unlockedLevelControl.unlockedLevelElements[i] != null && unlockedLevelControl.unlockedLevelElements[i].interagibleObject!= null
					    && comingLevel == unlockedLevelControl.unlockedLevelElements[i].levelNumber)
						beginPosition = unlockedLevelControl.unlockedLevelElements[i].interagibleObject.transform.position;
				}
			}
		}
	}

	void setPlayerPosition()
	{
		GeneralFinder.player.transform.position = beginPosition;
	}
}
