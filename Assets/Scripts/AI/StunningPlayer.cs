using UnityEngine;
using System.Collections;

public class StunningPlayer : MonoBehaviour {

	float tLastStun = -2.0f;
	float tBetweenAdvise = 2.0f;

	public void OnTriggerEnter2D(Collider2D c) {
		
		//Debug.Log ("schiacciato");

		if (c.gameObject.tag=="Player") {

			if(Time.time > tLastStun + tBetweenAdvise) {
				transform.parent.SendMessage ("playerStunned", true);
				tLastStun = Time.time;
			
			}
		} 

	}
}
