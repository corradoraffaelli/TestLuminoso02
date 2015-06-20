﻿using UnityEngine;
using System.Collections;

public class streetLampAnimation : MonoBehaviour {

	SpriteRenderer lampLight;
	SpriteRenderer lampConeLight;

	AudioHandler ah; 
	GameObject gameController;
	gameSet gs;

	private bool lampIsOn = false;

	// Use this for initialization
	void Start () {

		getLights ();
		getAudioHandler ();
	}

	private void getGameSet(){

		gameController = GameObject.FindGameObjectWithTag ("Controller");
		gs = gameController.GetComponent<gameSet> ();
		
	}

	private void getAudioHandler() {

		ah = GetComponent<AudioHandler> ();


		if (ah == null)
			Debug.Log ("ATTENZIONE - audio handler non attaccato al checkpoint");
	}

	private void getLights(){

		foreach (Transform child in transform) {

			lampLight = child.GetComponent<SpriteRenderer>();

			foreach(Transform nephew in child) {

				lampConeLight = nephew.GetComponent<SpriteRenderer>();

			}

		}

	}

	//--------------------------------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------

	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player" && !lampIsOn) {

			StartCoroutine( turnOnCheckpoint() );
			lampIsOn = true;

		}

	}

	private IEnumerator turnOnCheckpoint(){

		ah.playClipByName ("ClickActivating");

		lampLight.enabled = true;
		lampConeLight.enabled = true;

		for (int i=1; i<5; i++) {

			yield return new WaitForSeconds (0.05f + (0.3f / i));

			lampLight.enabled = false;
			lampConeLight.enabled = false;


			yield return new WaitForSeconds (0.05f + (0.3f / i));

			ah.playClipByName ("ClickActivating");
			lampLight.enabled = true;
			lampConeLight.enabled = true;

		}

	}
}