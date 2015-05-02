using UnityEngine;
using System.Collections;

public class sceneChanger : MonoBehaviour {

	public GameObject []toDisappear;
	public GameObject []toAppear;
	public bool defaultVerseRight = true;

	private GameObject controller;
	private GameObject respawnPoint;
	

	void Start () {

		tryInitializeSceneChanger ();
		takeSpriteRenderers ();
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

		//Debug.Log ("attivazione by " + gameObject.name);

		foreach(GameObject go in toDisappear) {
			if (go != null) {
				go.SetActive(false);
				//Debug.Log ("disattivato l'oggetto " + go.name);
			}
			
		}
		
		foreach(GameObject go in toAppear) {
			if (go != null) {
				go.SetActive(true);
				//Debug.Log ("attivato l'oggetto " + go.name);
			}
		}
		
		setRespawnPosition();

		this.gameObject.SetActive(false);
		
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {

			c_manualActivation();

		}

	}

	//-----------CORRADO----------------

	SpriteRenderer[][] spriteRenderers;
	void takeSpriteRenderers()
	{
		spriteRenderers = new SpriteRenderer[toAppear.Length][];

		for (int i = 0; i< spriteRenderers.Length; i++) {
			if (toAppear[i] != null) {
				spriteRenderers[i] = toAppear[i].GetComponentsInChildren<SpriteRenderer> ();
			}
		}

	}

}
