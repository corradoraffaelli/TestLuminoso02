using UnityEngine;
using System.Collections;

public class StunningPlayer : MonoBehaviour {

	bool DEBUG_STUNNING = false;

	float tLastStun = -2.0f;
	float tBetweenAdvise = 2.0f;
	public LayerMask targetLayers;
	basicAIEnemyV4 bai;

	void Start() {
		bai = transform.parent.gameObject.GetComponent<basicAIEnemyV4> ();
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
					transform.parent.SendMessage ("c_playerStunned", true);
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

		//bai.c_targetNear (n);
		transform.parent.gameObject.SendMessage ("c_targetNear", n);

	}
}
