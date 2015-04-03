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

		Debug.Log ("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

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

				if (c.gameObject.tag=="Player") {
					Debug.Log ("SALTATO MI HAI");
					if(DEBUG_STUNNED) {
						
						Debug.Log("Io sono " + transform.gameObject.name + " e sono stato colpito dal figlio di " + c.gameObject.transform.parent.gameObject.name);
						
					}

					transform.parent.SendMessage ("setStunned", true);

					//MODIFICA GESTIONE BOUNCE
					if(bouncy) {
						Rigidbody2D r = c.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
						r.velocity = new Vector2(0.0f, 0.0f);
						r.AddForce(new Vector2(0.0f, 350.0f));
						bouncy = false;
					}
				} 
				else {
					//Debug.Log ("nome oggetto " + c.gameObject.name);
				}

				break;

		}



	}

	public void c_setBouncy(bool b) {

		bouncy = b;

	}

}
