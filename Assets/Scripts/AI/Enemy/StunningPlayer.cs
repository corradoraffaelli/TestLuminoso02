using UnityEngine;
using System.Collections;

public class StunningPlayer : MonoBehaviour {

	bool DEBUG_STUNNING = false;

	float tLastStun = -2.0f;
	float tBetweenAdvise = 2.0f;
	basicAIEnemyV4 bai;
	public LayerMask targetLayers;

	void Start() {
		bai = transform.parent.GetComponent<basicAIEnemyV4> ();

	}

	public void OnTriggerEnter2D(Collider2D c) {
		
		//Debug.Log ("schiacciato");

		if ((targetLayers.value & 1 << c.gameObject.layer) > 0) {

			if (c.gameObject.tag == "Player") {

				if(DEBUG_STUNNING) {

					Debug.Log("Io sono " + transform.gameObject.name + " e sto colpendo il player " + c.gameObject.name);

				}

				i_targetNear (true);

				if (Time.time > tLastStun + tBetweenAdvise) {
					transform.parent.SendMessage ("playerStunned", true);
					tLastStun = Time.time;
			
				}
			}
			else {

				if(DEBUG_STUNNING) {
					
					Debug.Log("Io sono " + transform.gameObject.name + " e sto colpendo la proiezione " + c.gameObject.name);
					
				}

				i_targetNear (true);

			}
		}
	
	}

	public void OnTriggerExit2D(Collider2D c) {

		if ((targetLayers.value & 1 << c.gameObject.layer) > 0) {
			
			i_targetNear (false);

		}

	}

	public void i_targetNear(bool n) {

		bai.c_targetNear (n);

	}
}
