using UnityEngine;
using System.Collections;

public class GlassesUIManager : MonoBehaviour {

	GameObject MLLogic;
	MagicLantern MLScript;
	GlassesManager glassesManager;

	Glass[] usableGlassList;

	public GameObject GlassOnScreenPrefab;

	[Range(10.0f,200.0f)]
	public float xDistance = 20.0f;
	[Range(10.0f,200.0f)]
	public float yDistance = 20.0f;
	[Range(10.0f,200.0f)]
	public float xDistanceBetween = 30.0f;
	
	[Range(1.0f,2.0f)]
	public float enlargement = 1.3f;
	[Range(0.0f,1.0f)]
	public float alphaNotSelected = 0.3f;

	float xPrefabSize;
	float yPrefabSize;

	GameObject[] GlassPrefabList;
	
	int actualGlassIndex;

	void Start () {

		MLLogic = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
		MLScript = MLLogic.GetComponent<MagicLantern> ();
		glassesManager = MLLogic.GetComponent<GlassesManager> ();

		//salvo la larghezza e l'altezza dell'oggetto
		if (GlassOnScreenPrefab != null) {
			xPrefabSize = GlassOnScreenPrefab.GetComponent<RectTransform>().rect.width;
			yPrefabSize = GlassOnScreenPrefab.GetComponent<RectTransform>().rect.height;
		}

		//initialization
		reloadGlasses ();
		initializeGlassPrefabList ();
		setSpritesToPrefabs ();
		setGlassPositions ();
		setGlassSize ();
	
	}

	void Update () {

		if (Input.GetKeyUp (KeyCode.P)) {
			reloadGlasses ();
			initializeGlassPrefabList ();
			setSpritesToPrefabs ();
			setGlassPositions ();
			setGlassSize ();
		}

		if (actualGlassIndex != glassesManager.getActualGlassIndexUsableList ())
			setGlassSize ();

		if (usableGlassList != glassesManager.getUsableGlassList ()) {
			reinitialization ();
		}
		//reinitialization ();

	}

	void reinitialization(){
		reloadGlasses ();
		initializeGlassPrefabList ();
		setSpritesToPrefabs ();
		setGlassPositions ();
		setGlassSize ();
	}

	void reloadGlasses()
	{
		clearGlassesList ();
		getOnlyUsableGlasses ();
	}
	
	void changeActualGlass()
	{
		actualGlassIndex = glassesManager.getActualGlassIndexUsableList();
	}
	
	public void setGlassSize()
	{

		changeActualGlass ();
		if (usableGlassList!= null && GlassPrefabList != null && usableGlassList.Length == GlassPrefabList.Length) {
			for (int i = 0; i<usableGlassList.Length; i++)
			{
				if (i == actualGlassIndex)
				{
					GlassPrefabList[i].GetComponent<RectTransform>().localScale = new Vector3(enlargement,enlargement,1.0f);
					GlassPrefabList[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1.0f,1.0f,1.0f,1.0f);
				}
					
				else
				{
					GlassPrefabList[i].GetComponent<RectTransform>().localScale = new Vector3(1.0f,1.0f,1.0f);
					GlassPrefabList[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1.0f,1.0f,1.0f,alphaNotSelected);
				}
			}
		}

	}
	
	void setGlassPositions()
	{
		if (GlassPrefabList != null) {
			for (int i=0; i< GlassPrefabList.Length ; i++)
			{
				float tempIndex = GlassPrefabList.Length - i - 1;
				float xPosition = xDistance + (xPrefabSize/2) + (tempIndex*xPrefabSize) + (tempIndex*xDistanceBetween);
				GlassPrefabList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(- xPosition, yDistance);
			}
		}
	}
	
	void clearGlassesList()
	{
		if (usableGlassList != null)
			System.Array.Clear(usableGlassList,0,usableGlassList.Length);
	}


	void getOnlyUsableGlasses()
	{
		usableGlassList = glassesManager.getUsableGlassList ();


		/*
		int totalGlassNumber = MLScript.glassList.Length;
		int usableGlasses = 0;
		int index = 0;
		//trovo il numero di vetrini usabili
		for (int i = 0; i < totalGlassNumber; i++) {
			if (MLScript.glassList[i].Usable)
			{
				usableGlasses++;
			}
		}
		
		if (usableGlasses != 0) {
			//inizializzo l'array
			usableGlassList = new Glass[usableGlasses];
			//riempio l'array
			for (int i = 0; i < totalGlassNumber; i++) {
				if (MLScript.glassList[i].Usable)
				{
					usableGlassList[index] = MLScript.glassList[i];
					index++;
				}
			}
		}
		*/
	}

	
	void initializeGlassPrefabList()
	{
		if (GlassPrefabList != null)
		{
			//cancello tutti i prefab esistenti
			for (int i = 0; i< GlassPrefabList.Length; i++)
			{
				Destroy(GlassPrefabList[i]);
			}
			
			//pulisco la lista
			System.Array.Clear(GlassPrefabList,0,GlassPrefabList.Length);
		}
		
		//creo nuovi prefabs
		if (usableGlassList.Length != 0) {
			GlassPrefabList = new GameObject[usableGlassList.Length];
			for (int i = 0; i< usableGlassList.Length; i++)
			{
				GlassPrefabList[i] = Instantiate(GlassOnScreenPrefab);
				GlassPrefabList[i].transform.parent = transform;
			}
		}
		
	}
	
	void setSpritesToPrefabs()
	{
		
		if (usableGlassList!= null && GlassPrefabList != null && usableGlassList.Length == GlassPrefabList.Length) {
			for (int i = 0; i<GlassPrefabList.Length; i++)
			{
				GlassPrefabList[i].GetComponent<UnityEngine.UI.Image>().sprite = usableGlassList[i].spriteObject;
			}
		}
		
	}

}
