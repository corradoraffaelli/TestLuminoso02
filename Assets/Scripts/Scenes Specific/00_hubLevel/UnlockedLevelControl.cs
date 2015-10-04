using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Specifica dell'hub. Si occupa di gestire le porte di accesso ai livelli.
/// </summary>

// Corrado
public class UnlockedLevelControl : MonoBehaviour {

	public bool loadUnlocked = false;
	public float unlockDelay = 3.0f;
	public float disappearSpeed = 1.0f;
	public bool beginUnlockDoorsDelayed = false;

	[System.Serializable]
	public class UnlockedLevelInfo
	{
		public int levelNumber;
		public bool isUnlocked = false;
	}

	[System.Serializable]
	public class UnlockedLevelElements
	{
		public int levelNumber;
		public UnlockedLevelInfo unlockedLevelInfo;
		public GameObject externalDoor;
		public GameObject interagibleObject;
		public GameObject particleEffect;
		[HideInInspector]
		public InteragibileObject interagibleScript; 
		public SpriteRenderer externalRenderer;
		public GameObject particleObject;
		public bool needToUnlock;
	}

	[SerializeField]
	public UnlockedLevelElements[] unlockedLevelElements;
	public UnlockedLevelInfo[] unlockedLevelInfo;
	//InformativeSection contentSection;
	public string savedDoorFile = "savedDoor";
	public string savedFileExtention = ".dat";
	int savedInputFileIndex = 0;
	public int indexToLoad = 0;

	bool soundPlayed = false;
	AudioHandler audioHandler;
	public string unlockDoorSound = "UnlockDoor";

	void Start () {
		audioHandler = GetComponent<AudioHandler>();

		fillInteragibleScript();
		setSpriteRenderers();

		/*
		 * LOGICA:
		 * carico i dati salvati delle precedenti porte sbloccate nella struttura unlockedLevelInfo
		 * riempio i dati presenti in unlockedLevelElements con quelli dell'informativeManager
		 * controllo che non ci siano differenze tra le due strutture, il che significa che ho sbloccato una nuova porta
		 * comunque sia, devo sbloccare le porte che risultavano sbloccate anche prima, nel salvataggio
		 * agisco di conseguenza, una volta che ho visto che una porta deve essere sbloccata
		 * nell'OnDestroy salvo i dati della struttura unlockedLevelInfo
		 */

		//carico i dati salvati
		loadInfo();

		//setto i livelli che ho sbloccato nel frattempo come sbloccati, nella struttura unlockedLevelElements (sfrutto l'informative manager)
		setUnlocked();

		//per test posso azzerare i dati
		if (loadUnlocked)
			loadZeroes();

		//ULTIMA AGGIUNTA
		setFirstAlwaysUnlocked();

		//setto i livelli che ho sbloccato nel frattempo come sbloccati, nella struttura unlockedLevelElements
		//setUnlocked();

		//setto le porte e le variabili che non sono cambiate dall'ultima apertura del livello
		setBeginUnlocked();

		//controllo se c'è qualche differenza tra quelli salvati e quelli attuali, così da avviare le necessarie animazioni
		findDifferencies();

		//chiamo la funzione che si occupa di sbloccare le porte, dopo un delay iniziale
		Invoke("beginUnlockDoorsDelay", unlockDelay); 
	}

	void Update () {
		if (beginUnlockDoorsDelayed)
			unlockDoors();
	}

	void setUnlocked()
	{
		for (int i = 0; i < unlockedLevelElements.Length; i++)
		{
			if (unlockedLevelElements[i] != null && GeneralFinder.informativeManager.getActualCollectiblesSection(unlockedLevelElements[i].levelNumber) != null)
			{
				unlockedLevelElements[i].unlockedLevelInfo.levelNumber = unlockedLevelElements[i].levelNumber;
				InformativeSection contentSection = GeneralFinder.informativeManager.getActualCollectiblesSection(unlockedLevelElements[i].levelNumber);
				if (contentSection != null && !contentSection.lockedSection)
					unlockedLevelElements[i].unlockedLevelInfo.isUnlocked = true;
			}
		}
	}

	void findDifferencies()
	{
		if (unlockedLevelInfo != null)
		{
			for (int i = 0; i < unlockedLevelInfo.Length; i++)
			{
				if (unlockedLevelInfo[i] != null && !unlockedLevelInfo[i].isUnlocked)
				{
					UnlockedLevelElements unlockEl = findElemWithLevelNumber(unlockedLevelInfo[i].levelNumber);
					if (unlockEl != null && unlockEl.unlockedLevelInfo != null && unlockEl.unlockedLevelInfo.isUnlocked)
					{
						unlockEl.needToUnlock = true;
						Debug.Log ("trovata porta del livello " + unlockedLevelElements[i].levelNumber + "sbloccata"); 


					}
				}
			}
		}
	}

	void unlockDoors()
	{
		if (unlockedLevelElements != null && unlockedLevelElements.Length != 0)
		{
			for (int i = 0; i < unlockedLevelElements.Length; i++)
			{
				if (unlockedLevelElements[i] != null && unlockedLevelElements[i].needToUnlock && unlockedLevelElements[i].externalDoor != null)
				{
					//unlockedLevelElements[i].needToUnlock = false;
					//unlockedLevelElements[i].externalDoor.SetActive(false);
						unlockedLevelElements[i].interagibleObject.SetActive(true);

					//audioHandler
					if (audioHandler != null && !soundPlayed)
					{
						audioHandler.playClipByName(unlockDoorSound);
						soundPlayed = true;
					}

					//particles
					if (unlockedLevelElements[i].particleEffect != null)
						unlockedLevelElements[i].particleEffect.SetActive(true);

					if (unlockedLevelElements[i].externalRenderer != null)
					{
						Color actColor = unlockedLevelElements[i].externalRenderer.color;
						unlockedLevelElements[i].externalRenderer.color = new Color(actColor.r, actColor.g, actColor.b, Mathf.MoveTowards(actColor.a, 0.0f, Time.deltaTime * disappearSpeed));

						if (actColor.a == 0.0f)
						{
							unlockedLevelElements[i].needToUnlock = false;
							unlockedLevelElements[i].externalDoor.SetActive(false);
						}
							
					}
				}
			}
		}
	}

	void setBeginUnlocked()
	{
		if (unlockedLevelInfo != null && unlockedLevelElements != null)
		{
			for (int i = 0; i< unlockedLevelInfo.Length; i++)
			{
				if (unlockedLevelInfo[i] != null)
				{
					UnlockedLevelElements unlockedEl = findElemWithLevelNumber(unlockedLevelInfo[i].levelNumber);
					if (unlockedEl != null)
					{
						if (unlockedLevelInfo[i].isUnlocked)
						{
							unlockedEl.interagibleObject.SetActive(true);
							unlockedEl.externalDoor.SetActive(false);
						}
						else
						{
							unlockedEl.interagibleObject.SetActive(false);
							unlockedEl.externalDoor.SetActive(true);
						}
					}
				}
			}
		}
	}

	UnlockedLevelElements findElemWithLevelNumber(int inputNumber)
	{
		if (unlockedLevelElements != null)
		{
			for (int i = 0; i < unlockedLevelElements.Length; i++)
			{
				if (unlockedLevelElements[i] != null && unlockedLevelElements[i].levelNumber == inputNumber)
					return unlockedLevelElements[i];
			}
		}
		return null;
	}

	/*
	void OnDestroy()
	{
		saveInfo();
	}
	*/
	
	public void saveInfo()
	{
		unlockedLevelInfo = new UnlockedLevelInfo[unlockedLevelElements.Length];
		for (int i = 0; i < unlockedLevelInfo.Length; i++)
		{
			if (unlockedLevelElements[i] != null && unlockedLevelElements[i].unlockedLevelInfo != null)
				unlockedLevelInfo[i] = unlockedLevelElements[i].unlockedLevelInfo;
			else
				unlockedLevelInfo[i] = new UnlockedLevelInfo();
		}

		BinaryFormatter bf = new BinaryFormatter ();
		
		FileStream file = File.Create (Application.persistentDataPath + "/" +savedDoorFile + savedInputFileIndex+ savedFileExtention);
		bf.Serialize (file, unlockedLevelInfo);
		file.Close ();
	}


	void loadInfo()
	{
		if (File.Exists (Application.persistentDataPath + "/" +savedDoorFile + savedInputFileIndex+ savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/" +savedDoorFile + savedInputFileIndex+ savedFileExtention, FileMode.Open);
			
			unlockedLevelInfo = (UnlockedLevelInfo[])bf.Deserialize(file);

			file.Close();
		}
		else
		{
			loadUnlocked = true;
		}
	}

	void loadZeroes()
	{
		if (unlockedLevelInfo != null && unlockedLevelInfo.Length != 0)
		{
			for (int i = 0; i < unlockedLevelInfo.Length; i++)
			{
				if (unlockedLevelInfo[i] != null)
					unlockedLevelInfo[i].isUnlocked = false;
			}
		}else{
			unlockedLevelInfo = new UnlockedLevelInfo[unlockedLevelElements.Length];
			for (int i = 0; i < unlockedLevelInfo.Length; i++)
			{
				if (unlockedLevelElements[i] != null && unlockedLevelElements[i].unlockedLevelInfo != null)
				{
					unlockedLevelInfo[i] = new UnlockedLevelInfo();
					unlockedLevelInfo[i].levelNumber = unlockedLevelElements[i].unlockedLevelInfo.levelNumber;
					unlockedLevelInfo[i].isUnlocked = false;
				}
				else
					unlockedLevelInfo[i] = new UnlockedLevelInfo();
			}
		}
	}

	void setSpriteRenderers()
	{
		for (int i = 0; i < unlockedLevelElements.Length; i++)
		{
			if (unlockedLevelElements[i] != null && unlockedLevelElements[i].externalDoor != null)
			{
				unlockedLevelElements[i].externalRenderer = unlockedLevelElements[i].externalDoor.GetComponent<SpriteRenderer>();
			}
		}
	}

	void beginUnlockDoorsDelay()
	{
		beginUnlockDoorsDelayed = true;
	}

	void fillInteragibleScript()
	{
		for (int i = 0; i < unlockedLevelElements.Length; i++)
		{
			if (unlockedLevelElements[i] != null && unlockedLevelElements[i].interagibleObject != null)
				unlockedLevelElements[i].interagibleScript = unlockedLevelElements[i].interagibleObject.GetComponent<InteragibileObject>();
		}
	}

	void setFirstAlwaysUnlocked()
	{
		for (int i = 0; i < unlockedLevelElements.Length; i++)
		{
			if (unlockedLevelElements[i] != null && unlockedLevelElements[i].levelNumber == 1)
			{
				unlockedLevelElements[i].unlockedLevelInfo.isUnlocked = true;
				break;
			}
		}

		if (unlockedLevelInfo != null && unlockedLevelInfo.Length != 0)
		{
			for (int i = 0; i < unlockedLevelInfo.Length; i++)
			{
				if (unlockedLevelInfo[i] != null && unlockedLevelInfo[i].levelNumber == 1)
				{
					unlockedLevelInfo[i].isUnlocked = true;
					break;
				}
					
			}
		}
	}

}
