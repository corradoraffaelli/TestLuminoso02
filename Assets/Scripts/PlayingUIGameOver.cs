﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayingUIGameOver : MonoBehaviour {

	GameObject controller;
	GameObject canvasGameOver;
	// Use this for initialization

	void Start () {

		controller = GameObject.FindGameObjectWithTag ("Controller");

		foreach (Transform child in transform) {

			if(child.name=="GameOverScreen") {
				canvasGameOver = child.gameObject;
				break;
			}

		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void c_GameOver(float timeRespawn, float timeScale) {

		if (canvasGameOver != null){

			canvasGameOver.transform.SetAsLastSibling();

			StartCoroutine (handleGameOver (timeRespawn, timeScale));
		}
	}

	private IEnumerator handleGameOver(float timeRespawn, float timeScale) {
		
		Image sr = canvasGameOver.GetComponent<Image> ();
		sr.enabled = true;

		for (int i = 1; i<=20; i++) {
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/20);
			yield return new WaitForSeconds(0.01f);
			
		}

		controller.GetComponent<PlayStatusTracker> ().timeScale = timeScale;

		yield return new WaitForSeconds(timeRespawn);

		controller.GetComponent<PlayStatusTracker> ().timeScale = 1.0f;
		
		for (int i = 20; i>=0; i--) {
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/15);
			yield return new WaitForSeconds(0.01f);
			
		}
		
		sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.0f);
		sr.enabled = false;
	}

}