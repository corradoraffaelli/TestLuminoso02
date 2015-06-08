using UnityEngine;
using System.Collections;

public class sceneChanger : MonoBehaviour {

	public bool disableActualGameObject = true;
	public bool respawnPosition = true;

	public GameObject []toDisappear;
	public GameObject []toAppear;
	public bool defaultVerseRight = true;

	private GameObject controller;
	private GameObject respawnPoint;

	SpriteRenderer[][] spriteRenderersToAppear;
	SpriteRenderer[][] spriteRenderersToDisappear;
	GameObject[] tempDisappearGameObject;
	AudioHandler audioHandler;

	public float TimeToAppearDisappear = 2.0f;


	void Start () {

		tryInitializeSceneChanger ();

		//takeSpriteRenderers ();
		//changeSpriteRenderersAlpha (spriteRenderersToAppear, 0.0f);

		audioHandler = GetComponent<AudioHandler> ();

	}

	void Update()
	{
		changeAlphas ();
	}

	private void tryInitializeSceneChanger(){
		if (controller != null && respawnPoint != null) {
			return;
		}
		else {
			getGameController ();
			getRespawnPoint ();
		}

	}

	private void getGameController(){
		
		controller = GameObject.FindGameObjectWithTag ("Controller");
		
		if (controller == null)
			Debug.Log ("ATTENZIONE - oggetto GameController non trovato");
		
	}
	
	private void getRespawnPoint () {
		
		bool found = false;
		
		foreach (Transform child in controller.transform) {
			
			if (child.name == "RespawnPoint"){
				respawnPoint = child.gameObject;
				found = true; 
				break;
			}
			
		}
		
		if(!found)
			Debug.Log ("ATTENZIONE - oggetto RespawnPoint non trovato");
	}

	private void setRespawnPosition(bool recursive = false){
		
		if (respawnPoint != null) {
			
			respawnPoint.transform.position = transform.position;
			respawnPoint.transform.localScale = new Vector3 ((defaultVerseRight ? 1.0f : -1.0f), respawnPoint.transform.localScale.y, respawnPoint.transform.localScale.z);
		} 
		else {
			if(recursive) {
				Debug.Log ("ATTENZIONE - oggetto RespawnPoint non trovato neanche al secondo tentativo - by" + gameObject.name);
				return;
			}
			else {
				//TODO: per ora gestisco così il fatto che inizialmente possa essere disattivo...
				Debug.Log ("ATTENZIONE - oggetto RespawnPoint non trovato al primo tentativo - by" + gameObject.name);
				tryInitializeSceneChanger();

				setRespawnPosition(true);
			}
		}
		
	}

	public void c_manualActivation(){

		takeSpriteRenderers ();
		//Debug.Log ("attivazione by " + gameObject.name);
		/*
		foreach(GameObject go in toDisappear) {
			if (go != null) {
				go.SetActive(false);
				//Debug.Log ("disattivato l'oggetto " + go.name);
			}
			
		}
		*/
		foreach(GameObject go in toAppear) {
			if (go != null) {
				go.SetActive(true);
				//Debug.Log ("attivato l'oggetto " + go.name);
			}
		}

		if (respawnPosition)
			setRespawnPosition();

		//this.gameObject.SetActive(false);
		audioHandler.playClipByName ("Stella");
		hideGameObject();
		needChange = true;
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {

			c_manualActivation();

		}
		//audioHandler.playClipByName ("Comparsa");
	}

	//-----------CORRADO----------------

	void takeSpriteRenderers()
	{
		spriteRenderersToAppear = new SpriteRenderer[toAppear.Length][];

		for (int i = 0; i< spriteRenderersToAppear.Length; i++) {
			if (toAppear[i] != null) {
				spriteRenderersToAppear[i] = toAppear[i].GetComponentsInChildren<SpriteRenderer> (true);
			}
		}

		spriteRenderersToDisappear = new SpriteRenderer[toDisappear.Length][];
		//tempDisappearGameObject = new GameObject[toDisappear.Length];


		for (int i = 0; i< spriteRenderersToDisappear.Length; i++) {
			if (toDisappear[i] != null) {
				//disabilito tutti i component
				foreach (Behaviour childCompnent in toDisappear[i].GetComponentsInChildren<Behaviour>())
					childCompnent.enabled = false;

				//prendo solo i component sprite renderer e li attivo
				spriteRenderersToDisappear[i] = toDisappear[i].GetComponentsInChildren<SpriteRenderer> (true);
				for (int j=0;j<spriteRenderersToDisappear[i].Length;j++)
				{
					spriteRenderersToDisappear[i][j].enabled = true;
				}
			}
		}
	
	}

	void changeSpriteRenderersAlpha(SpriteRenderer[][] spriteRend, float newAlpha)
	{
		for (int i = 0; i<spriteRend.Length; i++)
		{
			if (spriteRend[i] != null)
			{
				for (int j = 0; j<spriteRend[i].Length;j++)
				{
					if (spriteRend[i][j] != null)
					{
						Color oldColor = spriteRend[i][j].color;
						spriteRend[i][j].color = new Color(oldColor.r,oldColor.g,oldColor.b, newAlpha);
					}

				}
			}

		}
	}

	float disappearAlpha = 1.0f;
	float appearAlpha = 0.0f;
	bool needChange = false;

	void changeAlphas()
	{
		if (needChange) {
			//calcolo i nuovi valori di alpha
			float deltaAlpha = Time.deltaTime / TimeToAppearDisappear;
			disappearAlpha = disappearAlpha - deltaAlpha;
			appearAlpha = appearAlpha + deltaAlpha;
			if (disappearAlpha < 0.0f)
				disappearAlpha = 0.0f;
			if (appearAlpha > 1.0f)
				appearAlpha = 1.0f;

			//cambio l'alpha
			changeSpriteRenderersAlpha(spriteRenderersToAppear, appearAlpha);
			changeSpriteRenderersAlpha(spriteRenderersToDisappear, disappearAlpha);

			//se ho finito di abilitare/disabilitare, disabilito l'oggetto e la lista di oggetti passata in ingresso
			if (appearAlpha == 1.0f && disappearAlpha == 0.0f) 
			{
				needChange = false;
				disactiveGameObjectsList();
				if (disableActualGameObject)
					disactiveGameObject();
			}
				
		}

	}

	void disactiveGameObjectsList()
	{
		foreach(GameObject go in toDisappear) {
			if (go != null) {
				go.SetActive(false);
			}
		}
	}

	void disactiveGameObject()
	{
		//this.gameObject.SetActive(false);
	}

	void hideGameObject()
	{
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;
	}
}
