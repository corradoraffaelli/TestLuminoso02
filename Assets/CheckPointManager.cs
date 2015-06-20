using UnityEngine;
using System.Collections;

public class CheckPointManager : MonoBehaviour {

	GameObject controller;
	GameObject respawnPoint;

	GameObject player;
	GameObject []checkpointObjs;
	CheckPointUpdate []checkpointScripts;

	bool initialized = false;

	// Use this for initialization
	void Start () {

		tryInitialization ();

	}

	#region STARTMETHODS

	private void tryInitialization(){

		if (initialized) 
			return;
		
		getPlayer ();
		getGameController ();
		getRespawnPoint ();
		getCheckpoints ();
		checkForGameStarterCheckpoint ();

		initialized = true;
		
	}

	void getPlayer() {

		player = UtilFinder._FindGameObjectByTag ("Player");

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

	void getCheckpoints() {

		checkpointObjs = GameObject.FindGameObjectsWithTag ("Checkpoint");
		checkpointScripts = new CheckPointUpdate[checkpointObjs.Length];

		int i = 0;

		foreach (GameObject ob in checkpointObjs) {

			checkpointScripts[i] = ob.GetComponent<CheckPointUpdate>();
			i++;

		}

	}

	void checkForGameStarterCheckpoint() {

		bool found = false;

		if (checkpointScripts != null) {

			foreach(CheckPointUpdate ch in checkpointScripts) {

				if(ch.gameStarterCheckpoint){

					if(!found) {

						ch.gameObject.SendMessage("c_manualCheckPointActivation");

						if(player!=null) {

							respawnPoint.transform.position = ch.respawnTransform.position;
							player.transform.position = respawnPoint.transform.position;

						}
						else {
							Debug.Log ("ATTENZIONE - player NULL in checkPointHandler");
						}
						found = true;
					}
					else {

						Debug.Log ("ATTENZIONE - più checkpoint sono settati come gameStarterPoint");

					}
				}

			}

		}

	}

	#endregion STARTMETHODS

	// Update is called once per frame
	void Update () {
	
	}
}
