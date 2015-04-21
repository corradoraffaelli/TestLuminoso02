﻿using UnityEngine;
using System.Collections;

public class sceneChanger : MonoBehaviour {

	public GameObject []toDisappear;
	public GameObject []toAppear;
	public bool defaultVerseRight = true;

	private GameObject controller;
	private GameObject respawnPoint;


	// Use this for initialization
	void Start () {
		getGameController ();
		getRespawnPoint ();

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

	private void setRespawnPosition(){
		
		if (respawnPoint != null) {
			
			respawnPoint.transform.position = transform.position;
			respawnPoint.transform.localScale = new Vector3( (defaultVerseRight ? 1.0f : -1.0f) ,respawnPoint.transform.localScale.y,respawnPoint.transform.localScale.z);
		}
		
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {

			foreach(GameObject go in toDisappear) {
				if (go != null)
					go.SetActive(false);

			}

			foreach(GameObject go in toAppear) {
				if (go != null)
					go.SetActive(true);
				
			}

			setRespawnPosition();

			this.gameObject.SetActive(false);

		}

	}

}
