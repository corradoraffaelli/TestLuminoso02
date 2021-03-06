﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce le particelle che vengono generate dalla Lanterna ogni volta che questa viene raccolta.
/// </summary>

// Corrado
public class ReturnParticles : MonoBehaviour {

	bool needToMove = false;
	bool moving = false;
	bool arrived = true;

	public float particlesVelocity = 0.3f;
	public float distanceFromPlayer = 0.1f;
	
	GameObject player;
	SpriteRenderer playerSpriteRenderer;

	GameObject lantern;

	ParticleSystem particleSystem;
	float defaultEmission;

	//GameObject lanternLogic;

	//Vector3 playerPosition;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		//lanternLogic = GameObject.FindGameObjectWithTag ("MagicLanternLogic");

		particleSystem = GetComponent<ParticleSystem> ();
		playerSpriteRenderer = player.GetComponent<SpriteRenderer> ();

		defaultEmission = particleSystem.emissionRate;
		particleSystem.emissionRate = 0;
		particleSystem.enableEmission = true;

		//Invoke ("activeParticles", 4.0f);
	}
	
	// Update is called once per frame
	void Update () {

		if (needToMove) {
		//lantern = GameObject.FindGameObjectWithTag ("Lantern");
			//if (lantern != null) {
			needToMove = false;	

			particleSystem.emissionRate = defaultEmission;
			moving = true;
				
			arrived = false;

				//transform.position = lanternLogic.GetComponent<MagicLantern>().lastLanternPosition;
				//playerPosition = playerSpriteRenderer.bounds.center;
			//} else {
				//Debug.Log ("ATTENZIONE! Probabilmente non hai settato il tag della lanterna a Lantern. Oppure al momento della chiamata, la lanterna non era attiva." +
				//	"Assicurati di chiamare il metodo quando la lanterna è ancora attiva.");
				//needToMove = false;
			///}
		}
			
		if (moving)
			transform.position = Vector3.Lerp (transform.position, playerSpriteRenderer.bounds.center, particlesVelocity * Time.deltaTime);
		
		if (Vector3.Distance (playerSpriteRenderer.bounds.center,transform.position) < distanceFromPlayer) {
			moving = false;
			arrived = true;
			particleSystem.emissionRate = 0;
		}


	}

	public void activeParticles(Vector3 startingPosition)
	{
		transform.position = startingPosition;
		needToMove = true;
	}

	public bool hasArrived()
	{
		return arrived;
	}
}
