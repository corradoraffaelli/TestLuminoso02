using UnityEngine;
using System.Collections;

public class StunningPlayer : MonoBehaviour {

	float tLastStun = -2.0f;
	float tBetweenAdvise = 2.0f;
	basicAIEnemyV4 bai;

	void Start() {
		bai = transform.parent.GetComponent<basicAIEnemyV4> ();

	}

	public void OnTriggerEnter2D(Collider2D c) {
		
		//Debug.Log ("schiacciato");

		if (c.gameObject.tag=="Player") {

			i_targetNear (true);

			if(Time.time > tLastStun + tBetweenAdvise) {
				transform.parent.SendMessage ("playerStunned", true);
				tLastStun = Time.time;
			
			}
		}
	
	}

	public void OnTriggerExit2D(Collider2D c) {

		if (c.gameObject.tag == "Player") {
			
			i_targetNear (false);

		}

	}

	public void i_targetNear(bool n) {

		bai.c_targetNear (n);

	}
}
