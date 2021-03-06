﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Classe generica che gestisce tutti gli oggetti interagibili.
/// </summary>

// Corrado
public class InteragibileObject : MonoBehaviour {
	public string methodToCall = "InteractingMethod";
	public GameObject[] objectsWithMethods;
	public bool oneTimeInteraction = false;
	public bool activeDisabledObject = true;
	//public float indicationScale = 2.0f;

	bool interacted = false;
	public bool playerColliding = false;
	GameObject player;
	GameObject indication;
	InteragibileObjectControllerSprite indicationScript;
	public bool updateScaleOnAppear = false;
	SpriteRenderer indicationRenderer;
	AudioHandler audioHandler;

	InputKeeper inputKeeper;

	public bool debugVar = false;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		indication = transform.GetChild (0).gameObject;
		if (indication != null) {
			indicationRenderer = indication.GetComponent<SpriteRenderer> ();
			indicationScript = indication.GetComponent<InteragibileObjectControllerSprite>();
		}
		//setIndicationScale ();
		audioHandler = GetComponent<AudioHandler> ();
		inputKeeper = GameObject.FindGameObjectWithTag ("Controller").GetComponent<InputKeeper> ();
		indication.SetActive (true);
	}

	void Update () {
		playerCollidingManagement ();

		if (debugVar && audioHandler.getAudioClipByName("Leva").audioSource.isPlaying)
			Debug.Log ("sto suonando");
	}

	void playerCollidingManagement()
	{

		if (!PlayStatusTracker.inPlay)
			return;

		if (((oneTimeInteraction && !interacted) || !oneTimeInteraction)) {
			//mostra la E
			setIndicationVisible(playerColliding);
			//indication.SetActive(playerColliding);

			//Debug.Log (gameObject.name +" "+ (inputKeeper!= null && inputKeeper.isButtonUp("Interaction") && ((oneTimeInteraction && !interacted) || !oneTimeInteraction) && playerColliding));
			//Debug.Log (gameObject.name +" "+ (((oneTimeInteraction && !interacted) || !oneTimeInteraction) && playerColliding));

			//scorre l'array di gameObject e chiama il metodo
			if (((oneTimeInteraction && !interacted) || !oneTimeInteraction) && playerColliding) {
				if (inputKeeper!= null && inputKeeper.isButtonDown("Interaction"))
				{
					//Debug.Log ("inviato messaggio di interazione ad oggetto ");
					if (audioHandler != null)
					{
						//Debug.Log ("i'm in");
						audioHandler.playClipByName("Leva");
					}
					
					
					interacted = true;
					
					for (int i = 0; i<objectsWithMethods.Length; i++)
					{
						if (objectsWithMethods[i] != null)
						{
							if (activeDisabledObject)
								objectsWithMethods[i].SetActive(true);
							objectsWithMethods[i].SendMessage(methodToCall,SendMessageOptions.DontRequireReceiver);
							
							//Debug.Log ("inviato messaggio di interazione ad oggetto "+objectsWithMethods[i].name);
						}
					}

					//gestione leva
					Lever leverScript = GetComponent<Lever>();
					if (leverScript != null)
						leverScript.InteractingMethod();

					//indication.SetActive(false);
					setIndicationVisible(false);
				}

			}
		}



	}

	public void c_setPlayerColliding(bool value) {

		playerColliding = value;

	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag.ToString() == "Player")
			playerColliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag.ToString() == "Player")
			playerColliding = false;
	}

	void setIndicationVisible(bool active = true)
	{
		if (indicationRenderer != null) {
			Color oldColor = indicationRenderer.color;
			float newAlpha;
			if (active)
			{
				if (updateScaleOnAppear && indicationScript != null)
					indicationScript.setIndicationScale();
				newAlpha = 1.0f;
			}
				
			else
				newAlpha = 0.0f;
			indicationRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);
		} else if (indication != null) {
			setIndicationVisible(active);
			//indication.SetActive(active);
		}
	}

	/*
	//setta la scala dell'indicazione (la E) nel world scale
	void setIndicationScale()
	{
		indication.transform.parent = null;
		indication.transform.localScale = new Vector3 (indicationScale, indicationScale, indicationScale);
		indication.transform.parent = transform;
	}
	*/
}
