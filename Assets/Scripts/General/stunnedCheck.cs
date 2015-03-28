using UnityEngine;
using System.Collections;

public class stunnedCheck : MonoBehaviour {

	public bool DEBUG_STUNNED = false;

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

				} 
				else {
					//Debug.Log ("nome oggetto " + c.gameObject.name);
				}

				break;

		}



	}
}
