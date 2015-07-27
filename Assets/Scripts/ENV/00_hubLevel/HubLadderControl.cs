using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class HubLadderControl : MonoBehaviour {

	public bool loadDefaultValues = true;
	public float appearLadderSpeed = 0.5f;

	[System.Serializable]
	public class LadderElement
	{
		public GameObject ladder;
		public bool unlocked;
		public bool toUnlock;
		public SpriteRenderer[] renderers;
	}

	[SerializeField]
	LadderElement[] ladderElement;

	bool[] ladderUnlocked;

	public string savedLadderFile = "savedLadder";
	public string savedFileExtention = ".dat";

	void Start () {
		//riempio gli sprite renderers
		getRenderers();

		//carico le informazioni salvate precdentemente
		loadInfo();

		//imposto la prima sempre come già sbloccata
		setFirstUnlocked();

		//se deciso, carico la situazione di default
		if (loadDefaultValues)
			setDefaultLadders();

		//setto le scale già sbloccate all'inizio
		setBeginUnlocked();

		copySaved();
	}

	void Update () {
		appearLadder();
	}

	/*
	void OnDestroy()
	{
		saveInfo();
	}
	*/

	public void IntaractingMethodMammut()
	{
		unlockNextLadder();
	}

	void unlockNextLadder()
	{
		for (int i = 0; i < ladderElement.Length; i++)
		{
			if (ladderElement[i] != null && !ladderElement[i].ladder.activeInHierarchy)
			{
				ladderElement[i].ladder.SetActive(true);
				setRenderersAlpha(ladderElement[i].renderers, 0.0f);
				ladderElement[i].unlocked = true;
				ladderElement[i].toUnlock = true;
				break;
			}
		}
	}

	void appearLadder()
	{
		for (int i = 0; i < ladderElement.Length; i++)
		{
			if (ladderElement[i] != null && ladderElement[i].toUnlock)
			{
				if (ladderElement[i].renderers != null && ladderElement[i].renderers[0] != null)
				{
					Color tempColor = ladderElement[i].renderers[0].color;
					float tempAlpha = Mathf.MoveTowards(tempColor.a, 1.0f, Time.deltaTime * appearLadderSpeed);
					setRenderersAlpha(ladderElement[i].renderers, tempAlpha);

					if (tempAlpha == 1.0f)
						ladderElement[i].toUnlock = false;
				}
			}
		}
	}

	void setRenderersAlpha(SpriteRenderer[] renderers, float inputAlpha)
	{
		if (renderers != null)
		{
			for (int i = 0; i< renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					Color  actColor = renderers[i].color;
					renderers[i].color = new Color(actColor.r, actColor.g, actColor.b, inputAlpha);
				}
			}
		}
	}

	public void saveInfo()
	{

		ladderUnlocked = new bool[ladderElement.Length];
		for (int i = 0; i < ladderUnlocked.Length; i++)
		{
			if (ladderElement[i] != null)
				ladderUnlocked[i] = ladderElement[i].unlocked;
			else
				ladderUnlocked[i] = false;
		}
		
		BinaryFormatter bf = new BinaryFormatter ();
		
		FileStream file = File.Create (Application.persistentDataPath + "/" +savedLadderFile + savedFileExtention);
		bf.Serialize (file, ladderUnlocked);
		file.Close ();

	}
	
	
	void loadInfo()
	{

		if (File.Exists (Application.persistentDataPath + "/" +savedLadderFile + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/" +savedLadderFile + savedFileExtention, FileMode.Open);
			
			ladderUnlocked = (bool[])bf.Deserialize(file);
		}else{
			loadDefaultValues = true;
		}

	}

	void setBeginUnlocked()
	{
		if (ladderUnlocked != null)
		{
			for (int i = 0; i < ladderUnlocked.Length; i++)
			{
				if (ladderElement[i] != null && ladderElement[i].ladder != null)
				{
					if (ladderUnlocked[i])
						ladderElement[i].ladder.SetActive(true);
					else
						ladderElement[i].ladder.SetActive(false);
				}
			}
		}
	}

	void setFirstUnlocked()
	{
		if (ladderUnlocked != null)
		{
			for (int i = 0; i < ladderUnlocked.Length; i++)
			{
				if (i == 0)
					ladderUnlocked[i] = true;
			}
		}

		if (ladderElement != null)
		{
			for (int i = 0; i < ladderElement.Length; i++)
			{
				if (i == 0 && ladderElement[i] != null)
					ladderElement[i].unlocked = true;
			}
		}
	}

	void setDefaultLadders()
	{
		ladderUnlocked = new bool[ladderElement.Length];
		for (int i = 0; i < ladderUnlocked.Length; i++)
		{
			if (i == 0)
				ladderUnlocked[i] = true;
			else
				ladderUnlocked[i] = false;
		}
	}

	void getRenderers()
	{
		for (int i = 0; i < ladderElement.Length; i++)
		{
			if (ladderElement[i] != null && ladderElement[i].ladder != null)
				ladderElement[i].renderers = ladderElement[i].ladder.GetComponentsInChildren<SpriteRenderer>();
		}
	}

	void copySaved()
	{
		if (ladderUnlocked!= null && ladderElement!= null && ladderUnlocked.Length == ladderElement.Length)
		{
			for (int i = 0; i< ladderUnlocked.Length; i++)
			{
				if (ladderElement[i] != null)
					ladderElement[i].unlocked = ladderUnlocked[i];
			}
		}
	}
}
