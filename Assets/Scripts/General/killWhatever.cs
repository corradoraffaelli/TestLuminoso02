using UnityEngine;
using System.Collections;

public class killWhatever : MonoBehaviour {

	public bool oneKill = false;
	public bool turnOn = true;
	public bool crusher = false;
	//private GameObject objectToCrush;
	private float tStartCrush = -1.0f;

	private GameObject exitingObj;


	public void OnCollisionEnter2D(Collision2D c) {

		if (!turnOn)
			return;

		if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy") {

			
			//Debug.Log ("entro");

			if(crusher) {

				//objectToCrush = c.gameObject;

				//Debug.Log ("parte coroutine");
				StartCoroutine(handleCrushKill(c.gameObject));
				//tStartCrush = Time.time;
			}

			else {
				//Debug.Log ("uccido e io sono" + gameObject.name);
				c.gameObject.SendMessage("c_instantKill", gameObject.tag);
				
				if(oneKill)
					turnOn = false;
			}

		}

	}

	private IEnumerator handleCrushKill(GameObject objectToKill) {
		//Debug.Log ("routine inizio");
		yield return new WaitForSeconds (0.1f);

		if (turnOn && objectToKill != null) {
			Debug.Log (objectToKill + "routine agisco");
			objectToKill.gameObject.SendMessage ("c_crushKill", gameObject.tag);
			//turnOn = false;
		}

		Debug.Log (objectToKill + "routine fine");

	}


	/*
	public void OnCollisionExit2D(Collision2D c) {

		if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy") {

			//Debug.Log ("esco");


			if (objectToCrush == null)
				return;


			if(crusher) {
				if (c.gameObject == objectToCrush)
					objectToCrush = null;
			}

		}
		
	}
	*/

}
