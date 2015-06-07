﻿using UnityEngine;
using System.Collections;
using System;

public class InteragibileObject : MonoBehaviour {
	public string methodToCall = "InteractingMethod";
	public GameObject[] objectsWithMethods;
	public bool oneTimeInteraction = false;
	public bool activeDisabledObject = true;
	public float indicationScale = 2.0f;

	bool interacted = false;
	public bool playerColliding = false;
	GameObject player;
	GameObject indication;
	AudioHandler audioHandler;

	InputKeeper inputKeeper;


	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		indication = transform.GetChild (0).gameObject;
		setIndicationScale ();
		audioHandler = GetComponent<AudioHandler> ();
		inputKeeper = GameObject.FindGameObjectWithTag ("Controller").GetComponent<InputKeeper> ();
	}

	void Update () {
		playerCollidingManagement ();
	}

	void playerCollidingManagement()
	{
		if (((oneTimeInteraction && !interacted) || !oneTimeInteraction)) {
			//mostra la E
			indication.SetActive(playerColliding);

			//Debug.Log (gameObject.name +" "+ (inputKeeper!= null && inputKeeper.isButtonUp("Interaction") && ((oneTimeInteraction && !interacted) || !oneTimeInteraction) && playerColliding));
			//Debug.Log (gameObject.name +" "+ (((oneTimeInteraction && !interacted) || !oneTimeInteraction) && playerColliding));

			//scorre l'array di gameObject e chiama il metodo
			if (((oneTimeInteraction && !interacted) || !oneTimeInteraction) && playerColliding) {
				if (inputKeeper!= null && inputKeeper.isButtonUp("Interaction"))
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
							
							Debug.Log ("inviato messaggio di interazione ad oggetto "+objectsWithMethods[i].name);
						}
					}
					indication.SetActive(false);
				}

			}
		}



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

	//setta la scala dell'indicazione (la E) nel world scale
	void setIndicationScale()
	{
		indication.transform.parent = null;
		indication.transform.localScale = new Vector3 (indicationScale, indicationScale, indicationScale);
		indication.transform.parent = transform;
	}
}
