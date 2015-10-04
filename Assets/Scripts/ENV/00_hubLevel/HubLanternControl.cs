using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Classe specifica dell'hub. Si occupa di gestire le lanterne mammut. Salvataggio e caricamento su file.
/// </summary>

// Corrado
public class HubLanternControl : MonoBehaviour {

	public bool setDefault = false;
	public string sortingLayerSceneUI = "SceneUI";

	[System.Serializable]
	public class MammuthElement
	{
		public string name;
		public InteragibileObject interagibleObject;
		[HideInInspector]
		public GameObject interagibleObjectOBJ;
		public GameObject triggerLimits;
		[HideInInspector]
		public SimplyVerifyIfPlayerInTrigger triggerScript;
		public GameObject starsInfo;
		public GameObject smokesFather;
		[HideInInspector]
		public GameObject[] smokes;
		public bool interacted;
		public bool canBeInteracted;
		public int starsNumber;
	}

	[SerializeField]
	MammuthElement[] mammuthElements;

	public GameObject[] bigProjections;
	SpriteRenderer[] bigProjectionsRenderers;

	int starsCollected;

	bool activatingProjection = false;
	SpriteRenderer rendererToActivate;
	public float appearProjectionSpeed = 0.2f;



	public string savedLanternFile = "savedMammut";
	public string savedFileExtention = ".dat";

	AudioHandler audioHandler;

	void Start () {
		fillTriggerScripts();
		fillNumbersOfIndications();
		takeRenderers();
		setProjectionAlpha(0.0f);
		loadInfo();
		if (setDefault)
			setDefaultStates();
		showProjectionsOnStart();
		HubFragmentCounter fragCounter = GetComponent<HubFragmentCounter>();
		if (fragCounter != null)
			starsCollected = fragCounter.fragmentTotalNumber;

		getSmokeChildren();
		setStartingSmoke();
		audioHandler = GetComponent<AudioHandler>();
		takeInteragibleOBJ();
	}

	void Update () {
		controlIfCanBeInteracted();
		handleActivatingProjection();
		showIndications();
	}

	//ad ogni update, controllo se la lanterna può essere interagita
	void controlIfCanBeInteracted()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].starsNumber <= starsCollected && !mammuthElements[i].interacted)
			{
				mammuthElements[i].canBeInteracted = true;
				//mammuthElements[i].interagibleObject.enabled = true;
				//mammuthElements[i].interagibleObject.SetActive(true);
				mammuthElements[i].interagibleObjectOBJ.SetActive(true);
			}
			else
			{
				mammuthElements[i].canBeInteracted = false;
				//mammuthElements[i].interagibleObject.enabled = false;
				//mammuthElements[i].interagibleObject.SetActive(false);
				mammuthElements[i].interagibleObjectOBJ.SetActive(false);
			}
				
		}
	}

	//metodo chiamato quando si interagisce con una lanterna
	public void IntaractingMethodMammut()
	{
		setLanternInteracted();
		activateNextProjection();
		activateSmoke();

		//NUOVO
		if (audioHandler != null)
			audioHandler.playClipByName("Accensione");
	}

	void getSmokeChildren()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].smokesFather != null)
			{
				int childNum = mammuthElements[i].smokesFather.transform.childCount;
				mammuthElements[i].smokes = new GameObject[childNum];
				for (int j = 0; j < childNum; j++)
				{
					mammuthElements[i].smokes[j] = mammuthElements[i].smokesFather.transform.GetChild(j).gameObject;
				}
				//mammuthElements[i].smokes = mammuthElements[i].smokesFather.transform.childCount
			}
			
		}
	}

	void activateSmoke()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].interagibleObject != null)
			{
				/*
				InteragibileObject intOBJScript = mammuthElements[i].interagibleObject.GetComponent<InteragibileObject>();
				if (intOBJScript != null && intOBJScript.playerColliding)
					//mammuthElements[i].interacted = true;
					mammuthElements[i].smokesFather.SetActive(true);
					*/
				if (mammuthElements[i].interagibleObject != null && mammuthElements[i].interagibleObject.playerColliding)
					//mammuthElements[i].interacted = true;
					mammuthElements[i].smokesFather.SetActive(true);
			}
		}
	}

	//dice qual è la prossima proiezione che deve essere attivata
	void activateNextProjection()
	{
		if (bigProjectionsRenderers != null)
		{
			for (int i = 0; i < bigProjectionsRenderers.Length; i++)
			{
				if (bigProjectionsRenderers[i] != null)
				{
					if (bigProjectionsRenderers[i].color.a == 0.0f)
					{
						rendererToActivate = bigProjectionsRenderers[i];
						activatingProjection = true;
						break;
					}
				}
			}
		}
	}

	//si occpua di rendere visibile la proiezione
	void handleActivatingProjection()
	{
		if (activatingProjection && rendererToActivate != null)
		{
			Color tempColor = rendererToActivate.color;
			float tempAlpha = Mathf.MoveTowards(tempColor.a, 1.0f, Time.deltaTime * appearProjectionSpeed);
			rendererToActivate.color = new Color(tempColor.r, tempColor.g, tempColor.b, tempAlpha);

			if (tempAlpha == 1.0f)
				activatingProjection = false;
		}
	}

	//prende gli sprite renderers delle proiezioni
	void takeRenderers()
	{
		bigProjectionsRenderers = new SpriteRenderer[bigProjections.Length];
		for (int i = 0; i < bigProjections.Length; i++)
		{
			if (bigProjections[i] != null)
				bigProjectionsRenderers[i] = bigProjections[i].GetComponent<SpriteRenderer>();
		}
	}

	//setta l'alpha di tutte le proiezioni
	void setProjectionAlpha(float inputAlpha)
	{
		if (bigProjectionsRenderers != null)
		{
			for (int i = 0; i < bigProjectionsRenderers.Length; i++)
			{
				if (bigProjectionsRenderers[i] != null)
				{
					Color tempColor = bigProjectionsRenderers[i].color;
					bigProjectionsRenderers[i].color = new Color(tempColor.r, tempColor.g, tempColor.b, inputAlpha);
				}
			}
		}
	}

	void setLanternInteracted()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].interagibleObject != null)
			{
				/*
				InteragibileObject intOBJScript = mammuthElements[i].interagibleObject.GetComponent<InteragibileObject>();
				if (intOBJScript != null && intOBJScript.playerColliding)
					mammuthElements[i].interacted = true;
					*/
				if (mammuthElements[i].interagibleObject != null && mammuthElements[i].interagibleObject.playerColliding)
					mammuthElements[i].interacted = true;
			}
		}
	}

	/*
	void OnDestroy()
	{
		saveInfo();
	}
	*/

	public void saveInfo()
	{

		bool[] lanternInteracted = new bool[mammuthElements.Length];
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null)
				lanternInteracted[i] = mammuthElements[i].interacted;
			else
				lanternInteracted[i] = false;
		}
		
		BinaryFormatter bf = new BinaryFormatter ();
		
		FileStream file = File.Create (Application.persistentDataPath + "/" +savedLanternFile + savedFileExtention);
		bf.Serialize (file, lanternInteracted);
		file.Close ();

	}
	
	
	void loadInfo()
	{

		if (File.Exists (Application.persistentDataPath + "/" +savedLanternFile + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/" +savedLanternFile + savedFileExtention, FileMode.Open);
			
			bool[] lanternInteracted = (bool[])bf.Deserialize(file);

			if (mammuthElements!= null && mammuthElements.Length == lanternInteracted.Length)
			{
				for (int i = 0; i < lanternInteracted.Length; i++)
				{
					if (mammuthElements[i] != null)
						mammuthElements[i].interacted = lanternInteracted[i];
				}
			}

			file.Close();
		}else{
			setDefault = true;
		}

	}

	void setDefaultStates()
	{
		if (mammuthElements!= null)
		{
			for (int i = 0; i < mammuthElements.Length; i++)
			{
				if (mammuthElements[i] != null)
					mammuthElements[i].interacted = false;
			}
		}
	}

	void showProjectionsOnStart()
	{
		if (mammuthElements!= null)
		{
			for (int i = 0; i < mammuthElements.Length; i++)
			{
				if (mammuthElements[i] != null && mammuthElements[i].interacted && bigProjectionsRenderers[i] != null)
				{
					Color tempColor = bigProjectionsRenderers[i].color;
					bigProjectionsRenderers[i].color = new Color (tempColor.r, tempColor.g, tempColor.b, 1.0f);
				}
					
			}
		}
	}

	void setStartingSmoke()
	{
		if (mammuthElements!= null)
		{
			for (int i = 0; i < mammuthElements.Length; i++)
			{
				if (mammuthElements[i] != null && mammuthElements[i].interacted && mammuthElements[i].smokesFather != null)
				{
					mammuthElements[i].smokesFather.SetActive(true);
				}
				
			}
		}
	}

	void fillTriggerScripts()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].triggerLimits != null)
				mammuthElements[i].triggerScript = mammuthElements[i].triggerLimits.GetComponent<SimplyVerifyIfPlayerInTrigger>();
		}
	}

	void fillNumbersOfIndications()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].starsInfo != null)
			{
				TextMesh textMesh = mammuthElements[i].starsInfo.GetComponent<TextMesh>();
				if (textMesh != null)
				{
					textMesh.text = mammuthElements[i].starsNumber.ToString();
				}
				MeshRenderer meshRenderer = mammuthElements[i].starsInfo.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					meshRenderer.sortingLayerName = sortingLayerSceneUI;
					meshRenderer.sortingOrder = 2;
				}
			}
		}
	}

	void showIndications()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].starsInfo != null && mammuthElements[i].triggerScript != null)
			{
				//if (mammuthElements[i].triggerScript.PlayerColliding)
				bool canBeEnabled = mammuthElements[i].triggerScript.PlayerColliding && !mammuthElements[i].interacted;
				mammuthElements[i].starsInfo.SetActive(canBeEnabled);
			}
		}
	}

	void takeInteragibleOBJ()
	{
		for (int i = 0; i < mammuthElements.Length; i++)
		{
			if (mammuthElements[i] != null && mammuthElements[i].interagibleObject != null)
			{
				mammuthElements[i].interagibleObjectOBJ = mammuthElements[i].interagibleObject.gameObject;
			}
		}
	}
}
