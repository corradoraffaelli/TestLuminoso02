using UnityEngine;
using System.Collections;

public class sizeChanger : MonoBehaviour {

	public enum sizeChangement {
		Smaller,
		Bigger,
	}

	public sizeChangement changement;

	public GameObject prevGameObject;

	public bool givePush = false;

	public float forceToApply = 50.0f;

	public GameObject EntrancePoint;
	public GameObject ExitPoint;

	bool tooEarly = false;
	float lastTime = 0.0f;
	float timeDiff = 0.5f;




	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		controlInTime ();
	}

	IEnumerator checkingCollider(Collider2D c) {

		//prevGameObject = c.gameObject;
		GameObject prevGameObjectTemp = c.gameObject;

		yield return new WaitForSeconds (5.0f);

		if(prevGameObjectTemp==prevGameObject)
			prevGameObject = null;

	}

	public void OnTriggerStay2D(Collider2D c) {

		if (c.tag == "Enemy" || c.tag == "Player") {
			if (givePush) {
			
				//c.attachedRigidbody.AddForce (Vector2.up * forceToApply);
				//c.attachedRigidbody.AddForce (
				//Vector3 dir = transform.TransformDirection(transform.localPosition);

				Transform par = transform.parent;

				//float zRot = par.localRotation.eulerAngles.z;
				//Vector3 basicDirection = Vector3.up;

				Vector3 vector = Quaternion.Euler(0, 0, par.localEulerAngles.z) * Vector3.up;
				/*
				float angle = par.eulerAngles.z * Mathf.Deg2Rad;
				float sin = Mathf.Sin( angle );
				float cos = Mathf.Cos( angle );
				
				Vector3 forward = new Vector3(
					direction.x * cos - direction.y * sin,
					direction.x * sin + direction.y * cos,
					0f );

				//Vector3 finalVerse = transform.TransformDirection(forward);

				forward = forward.normalized;
				*/

				vector = vector.normalized;

				c.attachedRigidbody.AddForce (vector * forceToApply);


				Debug.Log(vector.x + " " + vector.y +  " " + vector.z);

			}
		}
	}


	void controlInTime()
	{
		if (tooEarly) {
			if ((Time.time - lastTime) > timeDiff)
				tooEarly = false;
		}
	}

	public void OnTriggerExit2D(Collider2D c) {

		float factor = 1.0f;
		bool greater = false;


		if (ExitPoint != null && EntrancePoint != null) {

			Vector2 collV = new Vector2(c.gameObject.transform.position.x, c.gameObject.transform.position.y);

			Vector2 entranceV = new Vector2(EntrancePoint.transform.position.x,EntrancePoint.transform.position.y);

			Vector2 exitV = new Vector2(ExitPoint.transform.position.x,ExitPoint.transform.position.y);

			if( Mathf.Abs( Vector2.Distance(collV, exitV) ) > Mathf.Abs( Vector2.Distance(collV, entranceV) ) ) {

				return;

			}
			else {

				//Debug.Log("ahi1 - collV e exitV" + Mathf.Abs( Vector2.Distance(collV, exitV) ) + " ---  collV e entranceV " + Mathf.Abs( Vector2.Distance(collV, entranceV) ) );

			}

		}
		else {

		}

		if (tooEarly) {
			return;
		}

		tooEarly = true;
		lastTime = Time.time;

		if (c.tag == "Enemy" || c.tag == "Player") {

			switch(changement) {

			case(sizeChangement.Smaller):

				factor = 0.5f;
				greater = false;
				break;


			case(sizeChangement.Bigger):
				
				factor = 2.0f;
				greater = true;
				break;
			}

			c.gameObject.SendMessage("c_setPlayerMovementsFeatures", greater);

			if(c.tag == "Enemy") {

				setEnemyKillFeature (c, factor);

			}

			//Debug.Log("cambio " + c.gameObject.name);
			prevGameObject = c.gameObject;
			StartCoroutine (checkingCollider (c));

		}


	}

	void setEnemyKillFeature(Collider2D c, float factor) {

		if(c.transform.localScale.x<1.0f) {
			
			if(c.tag=="Enemy") {

				c.gameObject.SendMessage("c_setCanKillPlayer", false);
				
			}
			
		}
		else {
			
			if(c.tag=="Enemy") {

				c.gameObject.SendMessage("c_setCanKillPlayer", true);

			}
			
		}

	}
}
