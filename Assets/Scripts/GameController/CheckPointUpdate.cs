using UnityEngine;
using System.Collections;

public class CheckPointUpdate : MonoBehaviour {


	bool activated = false;
	public bool Activated {
		get{return activated; }

	}

	public bool respawnPosition = true;
	public bool defaultVerseRight = true;

	public bool gameStarterCheckpoint = false;
	
	private GameObject controller;
	private GameObject respawnPoint;

	public int optionalOrder = -1;

	public Transform respawnTransform {
		
		get {
			foreach(Transform child in transform) {

				if(child.name=="Light") {

					return child.transform;

				}


			}

			return transform;
			
		}
		
	}


	void Start () {
		
		tryInitializeSceneChanger ();
		
		//takeSpriteRenderers ();
		//changeSpriteRenderersAlpha (spriteRenderersToAppear, 0.0f);

	}
	
	void Update()
	{

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
	

	

	
	public void OnTriggerEnter2D(Collider2D c) {
		
		if (c.tag == "Player" && !activated) {
			Debug.Log ("trigger enter" + c.gameObject.name);
			c_manualActivation();

			CheckPointManager cpm = GameObject.FindGameObjectWithTag("Controller").GetComponent<CheckPointManager>();

			if(cpm!=null)
				cpm.c_setActiveLamp(this);
			else
				Debug.Log ("ATTENZIONE game controller non trovato opp CheckPointManager non trovato");

			activated = true;
		}

	}

	public void c_manualActivation(){
		
		if (respawnPosition)
			setRespawnPosition();
		
	}

	private void setRespawnPosition(bool recursive = false){
		
		if (respawnPoint != null) {
			//TODO : change
			//respawnPoint.transform.position = transform.position;
			respawnPoint.transform.position = respawnTransform.position;
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

}
