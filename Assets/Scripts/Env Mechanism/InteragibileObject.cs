using UnityEngine;
using System.Collections;
using System;

public class InteragibileObject : MonoBehaviour {
	public string methodToCall = "InteractingMethod";
	public GameObject[] objectsWithMethods;
	public bool oneTimeInteraction = false;
	public bool activeDisabledObject = true;
	public float indicationScale = 2.0f;

	bool interacted = false;
	bool playerColliding = false;
	GameObject player;
	GameObject indication;
	AudioHandler audioHandler;


	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		indication = transform.GetChild (0).gameObject;
		setIndicationScale ();
		audioHandler = GetComponent<AudioHandler> ();
	}

	void Update () {
		playerCollidingManagement ();
	}

	void playerCollidingManagement()
	{
		if (((oneTimeInteraction && !interacted) || !oneTimeInteraction)) {
			//mostra la E
			indication.SetActive(playerColliding);
			
			//scorre l'array di gameObject e chiama il metodo
			if (Input.GetButtonUp ("Interaction") && ((oneTimeInteraction && !interacted) || !oneTimeInteraction)) {

				if (audioHandler != null)
				{
					Debug.Log ("i'm in");
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

					}
				}
				indication.SetActive(false);
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
