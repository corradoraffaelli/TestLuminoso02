using UnityEngine;
using System.Collections;

public class CheckPointManager : MonoBehaviour {

	GameObject controller;
	GameObject respawnPoint;

	GameObject player;
	ArrayList checkpointObjs = new ArrayList ();
	ArrayList checkpointScripts = new ArrayList ();

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

		GameObject []temp = GameObject.FindGameObjectsWithTag ("Checkpoint");

		//checkpointScripts = new CheckPointUpdate[checkpointObjs.Length];

		foreach (GameObject ob in temp) {

			checkpointObjs.Add(ob);

			checkpointScripts.Add(ob.GetComponent<CheckPointUpdate>());


		}

	}

	void checkForGameStarterCheckpoint() {

		bool found = false;

		if (checkpointScripts.Count>0) {

			foreach(object chObj in checkpointScripts) {

				CheckPointUpdate ch = (CheckPointUpdate) chObj;

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

	#region PUBLICMETHODS

	public void c_setActiveLamp(CheckPointUpdate _checkpointScript) {

		//check if it exists
		Debug.Log ("set active lamp");
		bool found = false;

		foreach (object chObj in checkpointScripts) {

			CheckPointUpdate ch = (CheckPointUpdate) chObj;

			if(ch == _checkpointScript) {
				found = true;
				Debug.Log("found");
				break;
			}

		}

		if (!found) {
			Debug.Log("NOT found");
			checkpointScripts.Add(_checkpointScript);
			checkpointObjs.Add(_checkpointScript.gameObject);

		}

		foreach (object chObj in checkpointScripts) {
			
			CheckPointUpdate ch = (CheckPointUpdate) chObj;
			
			if(ch != _checkpointScript) {
				if(ch.Activated) {
					ch.gameObject.GetComponent<streetLampAnimation>().c_deactivateLight();
					ch.gameObject.GetComponent<CheckPointUpdate>().c_setInactiveLight();
					Debug.Log("chiamo deactivate su " + ch.gameObject.name);
				}

			}
			
		}

	}

	#endregion PUBLICMETHODS

	#endregion STARTMETHODS

	// Update is called once per frame
	void Update () {
	
	}
}
