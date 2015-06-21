using UnityEngine;
using System.Collections;

public class enemiesCleaner : MonoBehaviour {

	public bool DEBUG_CLEANING = false;

	public LayerMask cleanableLayers;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.gameObject.tag=="Enemy") {

			if(c.GetType() == typeof( CircleCollider2D) ) {
				//c.gameObject.GetComponent<basicAIEnemyV4>().c_startAutoDestroy(0.2f);
				if(c.gameObject.GetComponent<basicAIEnemyV4>()) {
					c.gameObject.GetComponent<basicAIEnemyV4>().c_startAutoDestroy(0.0f);
				}
				else {
					c.gameObject.GetComponent<AIAgent1>().c_autoDestroy();
				}
				if(DEBUG_CLEANING)
					Debug.Log ("Colliso con " + c.gameObject.name);
			}
			else {

				if(DEBUG_CLEANING)
					Debug.Log ("Colliso qualcosa che non è circle collider");

			}

		}
		else {

			if(DEBUG_CLEANING)
				Debug.Log ("Colliso qualcosa che non è tag enemy oppure non ha il layer enemies : " + c.gameObject.name);

		}

	}
}
