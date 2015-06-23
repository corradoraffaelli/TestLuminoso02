using UnityEngine;
using System.Collections;

public class cameraOscuraHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Player") {
			Debug.Log ("ciaone1");

			Rigidbody2D rb = c.gameObject.GetComponent<Rigidbody2D>();

			rb.gravityScale = -1;

			c.gameObject.transform.localScale = new Vector3( c.gameObject.transform.localScale.x, -Mathf.Abs( c.gameObject.transform.localScale.y), c.gameObject.transform.localScale.z);
			//c.gameObject.GetComponent<PlayerMovements>().gravityMultiplier = -1;


		}

	}

	public void OnTriggerExit2D(Collider2D c) {
		
		if (c.tag == "Player") {
			Debug.Log ("ciaone2");
			
			Rigidbody2D rb = c.gameObject.GetComponent<Rigidbody2D>();
			
			rb.gravityScale = 1;
			
			c.gameObject.transform.localScale = new Vector3( c.gameObject.transform.localScale.x, Mathf.Abs(c.gameObject.transform.localScale.y), c.gameObject.transform.localScale.z);
			//c.gameObject.GetComponent<PlayerMovements>().gravityMultiplier = -1;
			
			
		}
		
	}

}
