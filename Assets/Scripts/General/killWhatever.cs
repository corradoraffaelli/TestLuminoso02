using UnityEngine;
using System.Collections;

public class killWhatever : MonoBehaviour {

	public bool oneKill = false;
	public bool turnOn = true;
	public bool crusher = false;
	private GameObject objectToCrush;
	private float tStartCrush = -1.0f;
	


	public void OnCollisionEnter2D(Collision2D c) {

		if (!turnOn)
			return;

		if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy") {

			
			//Debug.Log ("entro");

			if(crusher) {
				objectToCrush = c.gameObject;
				Debug.Log ("parte coroutine");
				StartCoroutine(handleCrushKill());
				//tStartCrush = Time.time;
			}

			else {
				//Debug.Log ("uccido e io sono" + gameObject.name);
				c.gameObject.SendMessage("c_instantKill");
				
				if(oneKill)
					turnOn = false;
			}

		}

	}

	private IEnumerator handleCrushKill() {
		//Debug.Log ("routine inizio");
		yield return new WaitForSeconds (0.3f);

		if (turnOn && objectToCrush != null) {
			Debug.Log ("routine agisco");
			objectToCrush.gameObject.SendMessage ("c_crushKill");
			turnOn = false;
		}

		Debug.Log ("routine fine");

	}


	
	public void OnCollisionExit2D(Collision2D c) {

		if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy") {

			Debug.Log ("esco");


			if (objectToCrush == null)
				return;

			if(crusher) {
				if (c.gameObject == objectToCrush)
					objectToCrush = null;
			}
		}
		
	}
}
