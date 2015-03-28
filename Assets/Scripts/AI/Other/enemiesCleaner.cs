using UnityEngine;
using System.Collections;

public class enemiesCleaner : MonoBehaviour {

	public LayerMask cleanableLayers;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if ((cleanableLayers.value & 1 << c.gameObject.layer) > 0) {

			if(c.GetType() == typeof( CircleCollider2D) ) {
				c.gameObject.GetComponent<basicAIEnemyV4>().c_startAutoDestroy(0.2f);
				//Debug.Log ("Boom baby! : " + c.gameObject.name);
			}

		}

	}
}
