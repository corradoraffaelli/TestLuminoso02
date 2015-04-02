using UnityEngine;
using System.Collections;

public class stunnedCheck : MonoBehaviour {

	public bool DEBUG_STUNNED = false;
	public bool bouncy = true;

	public enum stunType {
		Player,
		AI,
	}

	public stunType IAM;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		//Debug.Log ("schiacciato");

		switch (IAM) {
			//questo script è di ciò che riceve lo stun

			case stunType.Player :

				if (c.gameObject.tag=="Stunning") {
					if(DEBUG_STUNNED) {
						
						Debug.Log("Io sono " + transform.gameObject.name + " e sono stato colpito dal figlio di " + c.gameObject.transform.parent.gameObject.name);

					}
					
					transform.SendMessage ("c_stunned", true);

					if(bouncy) {
						Vector2 dist = transform.position - c.gameObject.transform.position;
						
						Rigidbody2D r = GetComponent<Rigidbody2D>();
						r.velocity = new Vector2(0.0f, 0.0f);
						r.AddForce(300.0f*dist.normalized);

					}
					
				} 
				else {
					//Debug.Log ("nome oggetto " + c.gameObject.name);
				}
				break;

			case stunType.AI :

				if (c.gameObject.tag=="Stunning") {
					
					if(DEBUG_STUNNED) {
						
						Debug.Log("Io sono " + transform.gameObject.name + " e sono stato colpito dal figlio di " + c.gameObject.transform.parent.gameObject.name);
						
					}

					transform.parent.SendMessage ("setStunned", true);

					//c.gameObject.transform.parent.gameObject.SendMessage("c_bounce");
					//MODIFICA GESTIONE BOUNCE
					if(bouncy) {
						Rigidbody2D r = c.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
						r.velocity = new Vector2(0.0f, 0.0f);
						r.AddForce(new Vector2(0.0f, 350.0f));
						//TODO: gestire meglio il caso di non killable
						//caso che cmq non dovrebbe esserci, perché se gli salto addosso lo killo
						//se non lo killo mi dovrei far male io
						StartCoroutine(bouncyAgain(2.0f));
					}
				} 
				else {
					//Debug.Log ("nome oggetto " + c.gameObject.name);
				}

				break;

		}



	}

	private IEnumerator bouncyAgain(float timer) {

		bouncy = false;

		yield return new WaitForSeconds(timer);

		bouncy = true;
		
	}
}
