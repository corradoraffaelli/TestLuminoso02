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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator checkingCollider(Collider2D c) {

		prevGameObject = c.gameObject;
		GameObject prevGameObjectTemp = prevGameObject;

		yield return new WaitForSeconds (2.0f);

		if(prevGameObjectTemp==prevGameObject)
			prevGameObject = null;

	}

	public void OnTriggerStay2D(Collider2D c) {

		if (c.tag == "Enemy" || c.tag == "Player") {
			if (givePush) {
			
				c.attachedRigidbody.AddForce (Vector2.up * forceToApply);
			
			}
		}
	}

	public void OnTriggerExit2D(Collider2D c) {

		float factor = 1.0f;

		if (prevGameObject != null) {

			if (prevGameObject == c.gameObject)
				return;

		}

		if (c.tag == "Enemy" || c.tag == "Player") {

			switch(changement) {

			case(sizeChangement.Smaller):

				factor = 0.5f;

				break;


			case(sizeChangement.Bigger):
				
				factor = 2.0f;
				
				break;
			}

			c.transform.localScale = new Vector3(c.transform.localScale.x*factor, c.transform.localScale.y*factor, c.transform.localScale.z*factor);

			setEnemyFeatures (c, factor);


			StartCoroutine (checkingCollider (c));

		}


	}

	void setEnemyFeatures(Collider2D c, float factor) {

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
